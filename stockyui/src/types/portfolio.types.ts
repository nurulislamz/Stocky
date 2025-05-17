// Portfolio Summary
export interface PortfolioSummary {
  id: number;
  userId: number;
  totalValue: number;
  cashBalance: number;
  investedAmount: number;
  performance: {
    daily: number;
    weekly: number;
    monthly: number;
    yearly: number;
    allTime: number;
  };
  lastUpdated: Date;
}

// Individual Stock Holding
export interface StockHolding {
  id: number;
  portfolioId: number;
  symbol: string;
  shares: number;
  averageCost: number;
  currentPrice: number;
  marketValue: number;
  totalCost: number;
  gainLoss: number;
  gainLossPercentage: number;
  lastUpdated: Date;
}

// Portfolio Allocation
export interface PortfolioAllocation {
  sector: string;
  percentage: number;
  value: number;
}

// Portfolio Performance History
export interface PerformanceHistory {
  date: Date;
  value: number;
  change: number;
  changePercentage: number;
}

// Portfolio Transaction
export interface PortfolioTransaction {
  id: number;
  portfolioId: number;
  symbol: string;
  type: 'BUY' | 'SELL';
  shares: number;
  price: number;
  totalAmount: number;
  timestamp: Date;
  status: 'PENDING' | 'COMPLETED' | 'FAILED';
  orderType: 'MARKET' | 'LIMIT' | 'STOP';
  limitPrice?: number;
}

// Portfolio Dividend
export interface Dividend {
  id: number;
  portfolioId: number;
  symbol: string;
  amount: number;
  paymentDate: Date;
  exDate: Date;
  status: 'PENDING' | 'PAID';
}

// Portfolio Stats
export interface PortfolioStats {
  totalDividends: number;
  totalTrades: number;
  winRate: number;
  averageHoldingPeriod: number;
  bestPerformingStock: {
    symbol: string;
    gainPercentage: number;
  };
  worstPerformingStock: {
    symbol: string;
    lossPercentage: number;
  };
}

// Portfolio Goal
export interface PortfolioGoal {
  id: number;
  portfolioId: number;
  targetAmount: number;
  currentAmount: number;
  targetDate: Date;
  type: 'SAVINGS' | 'INVESTMENT' | 'RETIREMENT';
  status: 'IN_PROGRESS' | 'COMPLETED' | 'BEHIND';
}