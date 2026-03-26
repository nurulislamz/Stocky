# Plan: CQRS for the Event Store

## 1. Current State & Problems

### What exists today
```
PostgresEventStore          (writer — manages its own NpgsqlConnection)
  ├─ _connection            (opened in ctor, bypasses the factory)
  ├─ _reader                (IEventStoreReader, injected)
  └─ implements IEventStoreReader  ← problematic, writer IS-A reader

PostgresEventStoreReader    (reader — uses IDbConnectionFactory)
  └─ _factory.GetConnectionAsync()  ← always returns the WRITE connection

NpgsqlConnectionFactory     (scoped, has both connection strings)
  ├─ GetConnectionAsync()           → write connection
  └─ GetReadonlyConnectionAsync()   → read-only connection (UNUSED)

IDbConnectionFactory        (interface)
  └─ only exposes GetConnectionAsync()  ← no read-only method
```

### Problems
1. **PostgresEventStore manages its own connection** — bypasses the factory entirely, so DI scope lifetime doesn't control it.
2. **PostgresEventStore implements IEventStoreReader** — the writer IS-A reader, which violates CQRS. Consumers can't distinguish command vs query paths.
3. **IDbConnectionFactory has no read-only method** — `GetReadonlyConnectionAsync` exists on the concrete class but not the interface, so it's invisible to consumers.
4. **Reader always uses the write connection** — `PostgresEventStoreReader` calls `_factory.GetConnectionAsync()`, which returns the primary (read-write) connection. Reads never hit the read replica.
5. **No DI registrations for EventStore or Reader** — `Program.cs` registers `IEventRepository` → `EventRepository` (empty file), but nothing for `PostgresEventStore`, `IEventStoreReader`, or `IDbConnectionFactory`.

---

## 2. Target Architecture (ASCII Diagram)

```
┌──────────────────────────────────────────────────────────────────┐
│                        DI Container (Scoped)                     │
│                                                                  │
│  ┌─────────────────────────┐   ┌──────────────────────────────┐  │
│  │  IEventStoreWriter      │   │  IEventStoreReader           │  │
│  │  (PostgresEventStore)   │   │  (PostgresEventStoreReader)  │  │
│  │                         │   │                              │  │
│  │  • RegisterEventAsync   │   │  • QueryAllAggregatedEvents  │  │
│  │  • RegisterMultipleEvt  │   │  • QuerySingleEvent          │  │
│  │                         │   │  • GetStreamVersionAsync     │  │
│  │  uses ──────────────────┼──►│                              │  │
│  │  (composition, passes   │   │                              │  │
│  │   write conn when       │   │                              │  │
│  │   transactional read    │   │                              │  │
│  │   is needed)            │   │                              │  │
│  └────────────┬────────────┘   └──────────────┬───────────────┘  │
│               │                               │                  │
│               │ write conn                    │ read conn        │
│               ▼                               ▼                  │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │              IDbConnectionFactory                           │ │
│  │              (NpgsqlConnectionFactory)                      │ │
│  │                                                             │ │
│  │  GetWriteConnectionAsync()  → primary (read-write)         │ │
│  │  GetReadConnectionAsync()   → replica (read-only)          │ │
│  └─────────────────────────────────────────────────────────────┘ │
│               │                               │                  │
│               ▼                               ▼                  │
│         ┌──────────┐                   ┌──────────────┐          │
│         │ Postgres  │                   │ Postgres     │          │
│         │ Primary   │                   │ Read Replica │          │
│         └──────────┘                   └──────────────┘          │
│                                        (or same DB for now)      │
└──────────────────────────────────────────────────────────────────┘

Consumers (Builders, API handlers):
  ┌──────────────────────┐         ┌──────────────────────────────┐
  │  Command Handlers    │         │  EventStreamBuilder /        │
  │  (FundsApi, etc.)    │         │  Query Handlers              │
  │                      │         │                              │
  │  inject:             │         │  inject:                     │
  │   IEventStoreWriter  │         │   IEventStoreReader          │
  └──────────────────────┘         └──────────────────────────────┘
```

### Flow: Write path (command)
```
Handler → IEventStoreWriter.RegisterEventAsync(cmd, evt, ...)
  └─ writer gets write conn from factory
  └─ (row-versioning?) calls _reader.GetStreamVersionAsync(..., writeConn)
       └─ reader uses the passed writeConn (same txn)
  └─ executes INSERT via stored procedure on write conn
```

### Flow: Read path (query)
```
Handler → IEventStoreReader.QueryAllAggregatedEventsAsync(...)
  └─ reader gets read conn from factory (replica)
  └─ executes SELECT on read conn
```

### Flow: Writer needs a read during write (version check)
```
Writer calls _reader.GetStreamVersionAsync(aggType, aggId, _writeConn)
  └─ reader sees non-null connection → uses it (stays on write txn)
  └─ returns version
Writer proceeds with INSERT on same write conn
```

---

## 3. Is IDbConnectionFactory the right approach?

**Yes, but it needs two changes:**

| Aspect | Current | Target |
|--------|---------|--------|
| Interface methods | `GetConnectionAsync()` only | `GetWriteConnectionAsync()` + `GetReadConnectionAsync()` |
| Writer connection | Managed manually in ctor | Obtained from factory |
| Reader connection | Uses write connection | Uses read connection (or write when passed) |
| Lifetime | Factory is scoped; writer creates its own | Both use factory; scope owns all connections |

The factory pattern is the right abstraction because:
- It centralises connection lifetime management (scoped via DI)
- It naturally supports read/write splitting (you already have both connection strings)
- It avoids leaking `NpgsqlConnection` into constructors
- Future: swap to pooled `NpgsqlDataSource` without changing consumers

---

## 4. Implementation Steps

### Step 1: Split IDbConnectionFactory into read + write
```csharp
public interface IDbConnectionFactory
{
    ValueTask<NpgsqlConnection> GetWriteConnectionAsync(CancellationToken ct = default);
    ValueTask<NpgsqlConnection> GetReadConnectionAsync(CancellationToken ct = default);
}
```
Update `NpgsqlConnectionFactory` to implement both (rename existing methods).

### Step 2: Create IEventStoreWriter interface
```csharp
public interface IEventStoreWriter
{
    Task<CommandAndEventResult> RegisterEventAsync(...);
    Task<CommandAndEventResult[]> RegisterMultipleEventsAsync(...);
}
```
`PostgresEventStore` implements `IEventStoreWriter` only (remove `IEventStoreReader` from its type list).

### Step 3: Refactor PostgresEventStore to use the factory
- Remove `new NpgsqlConnection(connectionString)` from the constructor.
- Inject `IDbConnectionFactory` instead of a raw connection string.
- Call `_factory.GetWriteConnectionAsync()` to get the write connection.
- Keep injecting `IEventStoreReader` for version checks (composition).
- Move composite type mappings to a one-time startup helper or `NpgsqlDataSource` registration.

### Step 4: Update PostgresEventStoreReader to use read connection
- Change `_factory.GetConnectionAsync()` → `_factory.GetReadConnectionAsync()`.
- Keep the `IDbConnection? connection = null` override so the writer can pass its write connection.

### Step 5: Register everything in DI (Program.cs)
```csharp
// Connection factory (scoped — one per request)
services.AddScoped<IDbConnectionFactory>(sp =>
    new NpgsqlConnectionFactory(
        writeConnStr,
        readConnStr));

// Reader (scoped — uses factory's read connection)
services.AddScoped<IEventStoreReader, PostgresEventStoreReader>();

// Writer (scoped — uses factory's write connection + reader for version checks)
services.AddScoped<IEventStoreWriter, PostgresEventStore>();
```

### Step 6: Update consumers
- **Command handlers** (FundsApi, etc.): inject `IEventStoreWriter`.
- **Query handlers** (EventStreamBuilder, Builders): inject `IEventStoreReader` (already done).
- Remove any direct `PostgresEventStore` references.

### Step 7: Move composite type mappings to startup
The `NpgsqlDataSourceBuilder.MapComposite<T>()` calls currently live in the `PostgresEventStore` constructor. They should run once at app startup (e.g. in `Program.cs` or an extension method), not per-scope.

### Step 8: Clean up
- Delete the duplicate `ConcurrencyLevel` enum from `PostgresEventStore.cs` (already in `ConcurrencyType.cs`).
- Fix the broken constructor in `PostgresEventStore` (uses `await` in non-async ctor, references undefined variables).
- Remove `IEventStoreReader` from `PostgresEventStore`'s type list.

---

## 5. OOP Patterns Used

| Pattern | Where | Why |
|---------|-------|-----|
| **Interface Segregation** | `IEventStoreWriter` vs `IEventStoreReader` | Consumers depend only on the side they need |
| **Composition** | Writer *has-a* Reader | Writer delegates read-during-write to the reader |
| **Method Injection** | `IDbConnection? connection` param | Writer passes its write conn to reader for transactional reads |
| **Factory** | `IDbConnectionFactory` | Centralises connection creation and lifetime |
| **Scoped Lifetime** | DI registrations | One factory/writer/reader per request; connections disposed at scope end |

---

## 6. Future: Read Replicas / Eventual Consistency

Once this is in place, enabling read replicas is a config change:
- Set `readonlyConnectionString` to point at a Postgres streaming replica.
- Reads go to the replica automatically (eventual consistency).
- Writes + version checks stay on the primary (strong consistency).
- No code changes needed — the factory handles routing.
