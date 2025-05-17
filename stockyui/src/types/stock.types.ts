// Basic stock information
export interface StockBasicInfo {
  symbol: string;
  name: string;
  price: number;
  change: number;
  changePercent: number;
  volume: number;
  marketCap: number;
  lastUpdated: Date;
}

// For search results
export interface StockSearchResult {
  symbol: string;
  name: string;
  exchange: string;
  type: string;
  region: string;
  currency: string;
}

export interface StockEarnings {
  symbol: string;
  fiscalDateEnding: Date;
  reportedEPS: number;
  estimatedEPS: number;
  surprise: number;
  surprisePercentage: number;
  quarter: number;
  year: number;
}

export interface EarningsHistory {
  symbol: string;
  earnings: StockEarnings[];
  annualEarnings: {
    fiscalDateEnding: Date;
    reportedEPS: number;
  }[];
}

export interface StockNews {
  id: string;
  title: string;
  summary: string;
  url: string;
  publishedAt: Date;
  source: string;
  sentiment?: 'positive' | 'negative' | 'neutral';
  relatedSymbols: string[];
}

export interface NewsResponse {
  articles: StockNews[];
  totalResults: number;
  page: number;
}

export interface StockGeneralInfo {
  symbol: string;
  name: string;
  description: string;
  sector: string;
  industry: string;
  employees: number;
  website: string;
  address: string;
  phone: string;
  ceo: string;
  exchange: string;
  currency: string;
  country: string;
  isin: string;
  cusip: string;
  sedol: string;
}

export interface StockFinancials {
  symbol: string;
  metrics: {
    marketCap: number;
    enterpriseValue: number;
    peRatio: number;
    eps: number;
    dividendYield: number;
    beta: number;
    fiftyTwoWeekHigh: number;
    fiftyTwoWeekLow: number;
    fiftyDayAverage: number;
    twoHundredDayAverage: number;
  };
  balanceSheet: {
    totalAssets: number;
    totalLiabilities: number;
    totalEquity: number;
    currentRatio: number;
    debtToEquity: number;
  };
  incomeStatement: {
    revenue: number;
    grossProfit: number;
    operatingIncome: number;
    netIncome: number;
    ebitda: number;
  };
}