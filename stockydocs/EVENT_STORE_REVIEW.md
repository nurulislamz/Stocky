# Event Store Code Review

## Overview

The event store is implemented in two layers:

1. **EventRepository (EF Core)** – `stockyapi/Repository/Event/EventRepository.cs` + `IEventRepository`  
   Used by app code (User, Portfolio, Funds) to append events in the **same transaction** as read-model updates. Two-prong flow: `CreateEvent` → `Add(evt)` → caller `SaveChangesAsync()`.

2. **PostgresEventStore (Dapper)** – `stockymodels/EventStore/EventStore.cs`  
   Raw SQL append-only store with **command + event** in one transaction, optional optimistic vs advisory-lock concurrency. Not currently wired in DI (no `PostgresEventStore` in `Program.cs`); could be used for high-throughput or separate write path.

---

## Strengths

- **Append-only enforcement** – `ApplicationDbContext.EnforceEventStoreAppendOnly()` correctly blocks updates/deletes on `EventModel`.
- **Clear separation** – Events are immutable; only appends. `IEventRepository` is small and focused.
- **Two-prong flow** – Event + read-model in one transaction is documented and used consistently (e.g. UserRepository, PortfolioRepository, FundsRepository).
- **Concurrency options** – `PostgresEventStore` supports optimistic concurrency (retry on unique violation) and `LockOnAggregate` (advisory lock), with configurable retries and timeouts.
- **Schema** – `001CreateEventStore.sql` has a solid design: Commands table, Events with `(AggregateType, AggregateId, AggregateSequenceId)` unique, FK to Command, RLS on UserId, and bitemporal columns.
- **Bitemporal columns** – `TtStart`/`TtEnd`, `ValidFrom`/`ValidTo` support point-in-time and validity-time querying later.

---

## Issues and Bugs

### 1. PostgresEventStore – `QueryAllAggregatedEventsAsync` is wrong

- Uses `ExecuteScalarAsync<StockyEventPayload[]>` on a query that returns **multiple rows**.
- `ExecuteScalarAsync` returns a **single** value (first column of first row). It does not return an array of payloads.
- **Fix:** Use `QueryAsync<string>` (or similar) to read each `EventPayloadJson` row, then deserialize each to `StockyEventPayload` and return as array/list.

### 2. PostgresEventStore – `LogWarningAsync` does not exist

- `ILogger` has no `LogWarningAsync`. Standard API is `LogWarning(...)`.
- **Fix:** Use `_logger.LogWarning("...", retryAttempts, _retryAttempts)` (or equivalent overload).

### 3. PostgresEventStore – Optimistic retry and transaction rollback

- On unique violation (23505), the code retries but does not explicitly **roll back** the current transaction before the next attempt. Relying on `await using` disposal is fine, but an explicit `await transaction.RollbackAsync(ct)` in the catch block makes intent clear and is safer.

### 4. PostgresEventStore – `QuerySingleEventAsync` return type

- `ExecuteScalarAsync<StockyEventPayload>` on a JSONB column returns a single value; Dapper will not deserialize JSONB to `StockyEventPayload` automatically. You get the first column (e.g. string/JSON). So either:
  - Use `QuerySingleOrDefaultAsync<string>` and then `JsonSerializer.Deserialize<StockyEventPayload>(json)`, or
  - Use a Dapper type handler for JSONB → `StockyEventPayload`.

### 5. Schema vs model – `EventType` type mismatch

- **Migration** `001CreateEventStore.sql`: `"EventType" INTEGER NOT NULL`.
- **EventModel**: `EventType` is `string` (e.g. `eventPayload.GetType().Name` in `PostgresEventStore.CreateEventModel`).
- Either the table was later altered to VARCHAR, or there is a mismatch. If the column is still INTEGER, the Dapper INSERT will fail or require a mapping. Align migration and model (and stored procedure `003InsertCommandAndEventRaw.sql`, which uses `p_event_type INTEGER`).

### 6. Missing navigation properties (build errors)

- `EventConfiguration` uses `e.Command` and `CommandConfiguration` uses `c.Events`, but:
  - `EventModel` has no `Command` navigation property.
  - `CommandModel` has no `Events` collection.
- **Fix:** Add `CommandModel? Command { get; set; }` on `EventModel` and `ICollection<EventModel> Events { get; set; }` (or similar) on `CommandModel`, or remove/change the relationship configuration if you do not need navigation.

### 7. Repository sequence property name

- `PortfolioRepository.GetNextSequenceIdAsync` and `FundsRepository` use `e.SequenceId`.
- `EventModel` has **`AggregateSequenceId`**, not `SequenceId`.
- **Fix:** Use `e.AggregateSequenceId` in both repositories.

### 8. CommandPipeline – events not added in base

- Comment says: “Derived classes should add EventModels via EventRepository.Add() before calling base.” The base `ExecuteInTransactionAsync` only does `db.Update(projection)` and `SaveChangesAsync`; it never adds events. So every derived pipeline must add events before calling base. Consider having the base (or a shared helper) add events when the pipeline produces them, so the pattern is consistent and harder to forget.

---

## Suggested Additions (Functions / Capabilities)

### Event store (read side / replay)

| Function | Purpose |
|----------|--------|
| **GetStream(aggregateType, aggregateId, fromVersion?, toVersion?)** | Return ordered events for an aggregate (optionally from/to sequence). Essential for replay and building read models. |
| **GetEventsByUser(userId, since?, limit?)** | List events for a user (audit, debugging). |
| **GetEventsByCommand(commandId)** | Return all events produced by a command (command–event correlation). |
| **GetEventsByTraceId(traceId)** | Support distributed tracing and request correlation. |

### Event store (write side)

| Function | Purpose |
|----------|--------|
| **AppendBatch(CommandModel, EventModel[])** | One command → many events in a single transaction (e.g. user creation → UserCreated + PortfolioCreated + UserPreferencesCreated). |
| **Idempotent append (idempotency key)** | Optional idempotency by command id or (aggregate + expected version) to avoid duplicate processing. |

### Consistency and resilience

| Function | Purpose |
|----------|--------|
| **Health check** | Ping event store (e.g. “SELECT 1” or append to a heartbeat table) for startup and liveness probes. |
| **Optional outbox** | If you later send events to a message bus, an outbox table written in the same transaction as the event can ensure at-least-once publishing. |

### Projections and snapshots

| Function | Purpose |
|----------|--------|
| **Stream position / checkpoint** | Store “last processed event id” or “last sequence per aggregate” for projection builders. |
| **Snapshot storage** | For large aggregates, store periodic snapshots (e.g. state at sequence N) and replay only events after N. |

### IEventRepository interface

- **GetNextSequenceId(aggregateType, aggregateId)** – Today each repository implements its own `GetNextSequenceIdAsync` against `EventModels`. Moving this to `IEventRepository` would centralize sequence logic and make it reusable for any handler.
- **Optional: AppendBatchAsync(EventModel[])** – Append multiple events in one call while keeping a single transaction with read-model updates.

---

## Summary

- **EventRepository (EF)** is in good shape and fits the two-prong, same-transaction pattern. Fix: add missing `Command`/`Events` navigation properties (or adjust config), and use `AggregateSequenceId` in Portfolio and Funds repositories.
- **PostgresEventStore (Dapper)** has a clear design and concurrency options but: fix `QueryAllAggregatedEventsAsync` (use `QueryAsync` + deserialize), replace `LogWarningAsync` with `LogWarning`, and align `EventType` with the database. Then add stream/query APIs (GetStream, by user, by command, by trace) and consider batch append and idempotency for a more complete event store.
