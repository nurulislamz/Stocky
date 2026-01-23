Build started...
Build succeeded.
The Entity Framework tools version '9.0.4' is older than that of the runtime '10.0.2'. Update the tools for the latest features and bug fixes. See https://aka.ms/AAc1fbw for more information.
info: Startup[0]
      Using SQLite connection string Data Source=C:\Users\nurul\source\repos\Stocky\stockydb.db
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Users" (
    "UserId" TEXT NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY,
    "FirstName" TEXT NOT NULL,
    "Surname" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Password" TEXT NOT NULL,
    "Role" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "LastLogin" TEXT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE "Portfolios" (
    "PortfolioId" TEXT NOT NULL CONSTRAINT "PK_Portfolios" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "TotalValue" TEXT NOT NULL,
    "CashBalance" TEXT NOT NULL,
    "InvestedAmount" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_Portfolios_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "PriceAlerts" (
    "PriceAlertId" TEXT NOT NULL CONSTRAINT "PK_PriceAlerts" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "Symbol" TEXT NOT NULL,
    "TargetPrice" TEXT NOT NULL,
    "Condition" TEXT NOT NULL,
    "IsTriggered" INTEGER NOT NULL,
    "TriggeredAt" TEXT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_PriceAlerts_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "UserPreferences" (
    "UserPreferencesId" TEXT NOT NULL CONSTRAINT "PK_UserPreferences" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "Theme" INTEGER NOT NULL,
    "Currency" INTEGER NOT NULL,
    "Language" INTEGER NOT NULL,
    "EmailNotifications" INTEGER NOT NULL,
    "PushNotifications" INTEGER NOT NULL,
    "PriceAlerts" INTEGER NOT NULL,
    "NewsAlerts" INTEGER NOT NULL,
    "Timezone" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_UserPreferences_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "Watchlist" (
    "WatchlistId" TEXT NOT NULL CONSTRAINT "PK_Watchlist" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "Symbol" TEXT NOT NULL,
    "AddedAt" TEXT NOT NULL,
    "Notes" TEXT NULL,
    "TargetPrice" TEXT NULL,
    "StopLoss" TEXT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_Watchlist_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("UserId") ON DELETE CASCADE
);

CREATE TABLE "FundsTransactions" (
    "TransactionId" TEXT NOT NULL CONSTRAINT "PK_FundsTransactions" PRIMARY KEY,
    "PortfolioId" TEXT NOT NULL,
    "Type" INTEGER NOT NULL,
    "CashAmount" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_FundsTransactions_Portfolios_PortfolioId" FOREIGN KEY ("PortfolioId") REFERENCES "Portfolios" ("PortfolioId") ON DELETE CASCADE
);

CREATE TABLE "StockHoldings" (
    "StockHoldingId" TEXT NOT NULL CONSTRAINT "PK_StockHoldings" PRIMARY KEY,
    "PortfolioId" TEXT NOT NULL,
    "Ticker" TEXT NOT NULL,
    "Shares" TEXT NOT NULL,
    "AverageCost" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_StockHoldings_Portfolios_PortfolioId" FOREIGN KEY ("PortfolioId") REFERENCES "Portfolios" ("PortfolioId") ON DELETE CASCADE
);

CREATE TABLE "Transactions" (
    "TransactionId" TEXT NOT NULL CONSTRAINT "PK_Transactions" PRIMARY KEY,
    "PortfolioId" TEXT NOT NULL,
    "Ticker" TEXT NOT NULL,
    "Type" INTEGER NOT NULL,
    "Quantity" TEXT NOT NULL,
    "Price" TEXT NOT NULL,
    "NewAverageCost" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    "UpdatedAt" TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    CONSTRAINT "FK_Transactions_Portfolios_PortfolioId" FOREIGN KEY ("PortfolioId") REFERENCES "Portfolios" ("PortfolioId") ON DELETE CASCADE
);

CREATE INDEX "IX_FundsTransactions_PortfolioId" ON "FundsTransactions" ("PortfolioId");

CREATE UNIQUE INDEX "IX_Portfolios_UserId" ON "Portfolios" ("UserId");

CREATE INDEX "IX_PriceAlerts_Symbol_IsTriggered" ON "PriceAlerts" ("Symbol", "IsTriggered");

CREATE UNIQUE INDEX "IX_PriceAlerts_UserId_Symbol" ON "PriceAlerts" ("UserId", "Symbol");

CREATE UNIQUE INDEX "IX_StockHoldings_PortfolioId_Ticker" ON "StockHoldings" ("PortfolioId", "Ticker");

CREATE INDEX "IX_StockHoldings_Ticker" ON "StockHoldings" ("Ticker");

CREATE INDEX "IX_Transactions_PortfolioId_CreatedAt" ON "Transactions" ("PortfolioId", "CreatedAt");

CREATE UNIQUE INDEX "IX_UserPreferences_UserId" ON "UserPreferences" ("UserId");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Watchlist_UserId_Symbol" ON "Watchlist" ("UserId", "Symbol");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260123115052_InitialCreate', '10.0.2');

COMMIT;


