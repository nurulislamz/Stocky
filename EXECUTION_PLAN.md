# Stocky Event Store & Projections - Execution Plan

## Phase 1: Fix Compiler Errors (Immediate)

### 1.1 Fix EventStore.cs (stockymodels/EventStore/EventStore.cs)
- **Problem**: Broken syntax - `InsertEvent(StockyEventModel)` missing parameter type, `"""I )` invalid
- **Action**: Implement proper `InsertEvent(StockyEventModel evt)` using Dapper to insert into Events table per migration SQL
- **Alternative**: Remove/rewrite PostgresEventStore - it's a Dapper-based raw SQL store; ensure it matches 001CreateEventStore.sql schema

### 1.2 Fix ApplicationDbContext
- **Problem**: `EnforceEventStoreAppendOnly()` is called but not defined
- **Action**: Add the method (copy from stockyevents/EventStore or implement)

### 1.3 Fix EventConfiguration
- **Problem**: References `EventPayloadProtobuf` which EventModel doesn't have (migration SQL only has EventPayloadJson)
- **Action**: Remove EventPayloadProtobuf reference; align with migration SQL

### 1.4 Fix CommandConfiguration
- **Problem**: References `IssuedAt` which CommandModel doesn't have (migration SQL has no IssuedAt)
- **Action**: Remove IssuedAt index

### 1.5 Fix EventRepository
- **Problem**: EventModel.Id is Int64, CreateEvent uses Guid.NewGuid(); EventModel has no EventPayloadProtobuf
- **Action**: Align EventModel creation with schema (Id is BIGSERIAL/auto; remove Protobuf)

### 1.6 Fix stockymodels.csproj
- **Problem**: Duplicate Npgsql.EntityFrameworkCore.PostgreSQL package reference
- **Action**: Remove duplicate

## Phase 2: Align Models with Migration SQL (001CreateEventStore.sql)

### 2.1 EventModel
Migration SQL columns: EventId, UserId, AggregateType, AggregateTypeDesc, AggregateId, SequenceId, AggregateVersion, EventType, EventPayloadJson, TtStart, TtEnd, ValidFrom, ValidTo, CommandId, TraceId

- Add: UserId, AggregateTypeDesc, CommandId (for FK)
- Remove: EventPayloadProtobuf (not in SQL)
- EventPayloadJson: StringLength(400) may be too small for JSONB - use max or remove limit
- Id: BIGSERIAL = auto-generated, so don't set in code

### 2.2 CommandModel
Migration SQL: CommandId, UserId, CommandType, CommandPayloadJson, RequestId, TraceId
- Add: TraceId (nullable)
- CommandPayloadJson: Use JSONB type in config if needed

### 2.3 AggregateType enum
SQL uses INTEGER. Add UserId to enum (docs mention User aggregate).

## Phase 3: Create Projection Tables Migration

Per DATABASE_ARCHITECTURE.md, projections (read models) are:
- Users
- Portfolios  
- StockHoldings
- Watchlist
- UserPreferences
- PriceAlerts

Create new migration `002CreateProjections.sql` that creates these tables in stockydb schema, with proper FKs. Follow existing EF configurations for column types.

## Phase 4: Schema & Migration Setup

### 4.1 Configure schema
- All tables in stockydb schema (SET search_path TO stockydb in migration)
- Ensure EF configurations use schema where needed

### 4.2 Embed SQL in migration
- 001CreateEventStore already uses embedded SQL - keep that pattern
- 002CreateProjections: New migration class that runs 002CreateProjections.sql

## Phase 5: Git Branch

- Create branch `feature/event-store-projections` (or similar)
- Ensure clean build, no compiler errors
- Ready for merge/rebase into main
