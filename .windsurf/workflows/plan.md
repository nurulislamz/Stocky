# Plan: Refactor Event Store & Reader Separation

## Objective
Separate the **Write** responsibilities (EventStore) from the **Read** responsibilities (EventStoreReader) while allowing the Writer to leverage Reader logic for consistency checks (e.g., stream version validation).

## Current State
- `PostgresEventStore`: Handles writes (`RegisterEventAsync`) AND reads (`QueryAllAggregatedEventsAsync`). Manages its own `NpgsqlConnection`.
- `PostgresEventStoreReader`: Handles reads. Uses `IDbConnectionFactory`.

## Proposed Architecture
Use **Composition** over Inheritance. The `EventStore` should depend on `IEventStoreReader` to perform read operations required during the write process (like checking stream versions).

To enable this, we must solve the **Transaction Sharing** problem: The Reader must be able to execute queries on the Writer's active transaction/connection to ensure consistency.

## Steps

### 1. Enhance IEventStoreReader
Add methods required by the Writer, such as:
- `GetStreamVersionAsync(string aggregateType, Guid aggregateId, IDbTransaction? transaction = null)`

*Note: The optional `IDbTransaction` parameter allows the method to participate in an existing transaction.*

### 2. Refactor PostgresEventStoreReader
- Implement `GetStreamVersionAsync`.
- Update existing read methods to accept an optional `IDbConnection` or `IDbTransaction` if they might be used within a write context.
- Ensure it uses the provided connection/transaction if present, otherwise falls back to `IDbConnectionFactory`.

### 3. Refactor PostgresEventStore
- **Inject** `IEventStoreReader` into the constructor.
- **Remove** `QueryAllAggregatedEventsAsync` and `QuerySingleEventAsync` from `PostgresEventStore`.
- Update `RegisterEventAsync` (and specifically `WithRowVersioning`) to use `_reader.GetStreamVersionAsync()` instead of executing raw SQL directly.
- Pass the current `_connection` (or active transaction) to the Reader methods.

### 4. Cleanup
- Remove the raw SQL definitions for reading versions from `PostgresEventStore` if they are now encapsulated in the Reader.

## OOP Patterns Used
- **Composition**: `PostgresEventStore` *has-a* `IEventStoreReader`.
- **Method Injection**: Passing the context (`IDbTransaction`) to the dependency's methods to allow shared scope.
