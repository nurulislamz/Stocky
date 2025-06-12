import { AuthService } from './auth';

// Types
export interface PortfolioItem {
  symbol: string;
  quantity: number;
  averageBuyPrice: number;
  currentPrice: number;
  totalValue: number;
  profitLoss: number;
  profitLossPercentage: number;
  lastUpdatedTime: string;
}

export interface PortfolioData {
  totalValue: number;
  cashBalance: number;
  investedAmount: number;
  items: PortfolioItem[];
}

export interface UserPortfolioResponse {
  success: boolean;
  statusCode: number;
  message?: string;
  data?: PortfolioData;
}

export interface BuyTickerRequest {
  symbol: string;
  quantity: number;
  price: number;
}

export interface BuyTickerData {
  symbol: string;
  quantity: number;
  price: number;
  totalCost: number;
  remainingCashBalance: number;
  transactionTime: string;
  transactionId: string;
  status: string;
}

export interface BuyTickerResponse {
  success: boolean;
  statusCode: number;
  message?: string;
  data?: BuyTickerData;
}

export interface SellTickerRequest {
  symbol: string;
  quantity: number;
  price: number;
}

export interface SellTickerData {
  symbol: string;
  quantity: number;
  price: number;
  totalValue: number;
  remainingCashBalance: number;
  transactionTime: string;
  transactionId: string;
  status: string;
}

export interface SellTickerResponse {
  success: boolean;
  statusCode: number;
  message?: string;
  data?: SellTickerData;
}

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export const PortfolioService = {
  async getUserPortfolio(): Promise<UserPortfolioResponse> {
    try {
      const token = AuthService.getToken();
      if (!token) {
        throw new Error('Not authenticated');
      }

      const response = await fetch(`${API_BASE_URL}/portfolio/portfolio`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error fetching portfolio:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to fetch portfolio',
      };
    }
  },

  async buyTicker(request: BuyTickerRequest): Promise<BuyTickerResponse> {
    try {
      const token = AuthService.getToken();
      if (!token) {
        throw new Error('Not authenticated');
      }

      const response = await fetch(`${API_BASE_URL}/portfolio/buy`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      });

      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error buying ticker:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to buy ticker',
      };
    }
  },

  async sellTicker(request: SellTickerRequest): Promise<SellTickerResponse> {
    try {
      const token = AuthService.getToken();
      if (!token) {
        throw new Error('Not authenticated');
      }

      const response = await fetch(`${API_BASE_URL}/portfolio/sell`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      });

      const data = await response.json();
      return data;
    } catch (error) {
      console.error('Error selling ticker:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to sell ticker',
      };
    }
  },
};