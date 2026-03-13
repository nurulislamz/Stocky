-- Projection tables (read models) derived from event store.
-- These are updated by command handlers in the two-prong flow.
SET search_path TO stockydb;

-- Users: projection from UserCreated, UserNameChanged, UserEmailChanged, etc.
CREATE TABLE IF NOT EXISTS "Users" (
    "UserId" UUID PRIMARY KEY,
    "FirstName" VARCHAR(50) NOT NULL,
    "Surname" VARCHAR(50) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "Role" INTEGER NOT NULL DEFAULT 0,
    "IsActive" BOOLEAN NOT NULL DEFAULT true,
    "LastLogin" TIMESTAMPTZ NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX IF NOT EXISTS "ix_users_email" ON "Users" ("Email");

-- Portfolios: projection from PortfolioCreated, FundsDeposited, StockBought, etc.
CREATE TABLE IF NOT EXISTS "Portfolios" (
    "PortfolioId" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL UNIQUE,
    "TotalValue" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "CashBalance" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "InvestedAmount" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "fk_portfolios_users" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

-- StockHoldings: projection from HoldingCreation, HoldingModification, HoldingDeletion
CREATE TABLE IF NOT EXISTS "StockHoldings" (
    "StockHoldingId" UUID PRIMARY KEY,
    "PortfolioId" UUID NOT NULL,
    "Ticker" VARCHAR(20) NOT NULL,
    "Shares" DECIMAL(18,4) NOT NULL,
    "AverageCost" DECIMAL(18,2) NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "fk_stockholdings_portfolios" FOREIGN KEY ("PortfolioId") REFERENCES "Portfolios" ("PortfolioId") ON DELETE CASCADE,
    CONSTRAINT "uq_stockholdings_portfolio_ticker" UNIQUE ("PortfolioId", "Ticker")
);
CREATE INDEX IF NOT EXISTS "ix_stockholdings_ticker" ON "StockHoldings" ("Ticker");

-- UserPreferences: projection from UserPreferencesCreated
CREATE TABLE IF NOT EXISTS "UserPreferences" (
    "UserPreferencesId" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL UNIQUE,
    "Theme" INTEGER NOT NULL DEFAULT 0,
    "Currency" INTEGER NOT NULL DEFAULT 0,
    "Language" INTEGER NOT NULL DEFAULT 0,
    "EmailNotifications" BOOLEAN NOT NULL DEFAULT true,
    "PushNotifications" BOOLEAN NOT NULL DEFAULT true,
    "PriceAlerts" BOOLEAN NOT NULL DEFAULT true,
    "NewsAlerts" BOOLEAN NOT NULL DEFAULT true,
    "Timezone" VARCHAR(10) NOT NULL DEFAULT 'UTC',
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "fk_userpreferences_users" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

-- Watchlist: projection from WatchlistItemAdded, WatchlistItemRemoved
CREATE TABLE IF NOT EXISTS "Watchlist" (
    "WatchlistId" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "Symbol" VARCHAR(20) NOT NULL,
    "AddedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Notes" VARCHAR(500) NULL,
    "TargetPrice" DECIMAL(18,2) NULL,
    "StopLoss" DECIMAL(18,2) NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "fk_watchlist_users" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE,
    CONSTRAINT "uq_watchlist_user_symbol" UNIQUE ("UserId", "Symbol")
);

-- PriceAlerts: projection (user-defined alerts)
CREATE TABLE IF NOT EXISTS "PriceAlerts" (
    "PriceAlertId" UUID PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "Symbol" VARCHAR(20) NOT NULL,
    "TargetPrice" DECIMAL(18,2) NOT NULL,
    "Condition" VARCHAR(10) NOT NULL,
    "IsTriggered" BOOLEAN NOT NULL DEFAULT false,
    "TriggeredAt" TIMESTAMPTZ NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "fk_pricealerts_users" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE,
    CONSTRAINT "uq_pricealerts_user_symbol" UNIQUE ("UserId", "Symbol")
);
CREATE INDEX IF NOT EXISTS "ix_pricealerts_symbol_triggered" ON "PriceAlerts" ("Symbol", "IsTriggered");
