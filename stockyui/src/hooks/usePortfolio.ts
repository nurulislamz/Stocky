import { useState, useEffect } from 'react';
import { PortfolioService } from '../services/portfolio.service';
import { StockyApi } from '../services/generated/stockyapi';

interface PortfolioInfo {
  items?: StockyApi.PortfolioItem[] | undefined;
  totalValue?: number;
  cashBalance?: number;
  investedAmount?: number;
  isLoading: boolean;
  error?: string;
}

export const usePortfolio = () => {
  const portfolioService = new PortfolioService();
  const [portfolioState, setPortfolioState] = useState<PortfolioInfo>({
    items: [],
    totalValue: 0,
    cashBalance: 0,
    investedAmount: 0,
    isLoading: true,
    error: undefined
  });

  const fetchPortfolio = async () => {
    try {
      setPortfolioState(prev => ({ ...prev, isLoading: true, error: undefined }));
      const response = await portfolioService.getPortfolio();

      if (response.success && response.data) {
        setPortfolioState({
          items: response.data.items,
          totalValue: response.data.totalValue,
          cashBalance: response.data.cashBalance,
          investedAmount: response.data.investedAmount,
          isLoading: false,
          error: undefined
        });
      } else {
        setPortfolioState(prev => ({
          ...prev,
          isLoading: false,
          error: response.message || 'Failed to fetch portfolio'
        }));
      }
    } catch (error) {
      setPortfolioState(prev => ({
        ...prev,
        isLoading: false,
        error: 'Failed to fetch portfolio'
      }));
    }
  };

  const buyTicker = async (symbol: string, quantity: number, price: number) => {
    try {
      const request = new StockyApi.BuyTickerRequest({
        symbol: symbol,
        quantity: quantity,
        price: price
      });

      const response = await portfolioService.buyTicker(request);

      if (response.success) {
        // Refresh portfolio after successful purchase
        await fetchPortfolio();
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Failed to buy ticker' };
      }
    } catch (error) {
      return { success: false, message: 'Failed to buy ticker' };
    }
  };

  const sellTicker = async (symbol: string, quantity: number, price: number) => {
    try {
      const request = new StockyApi.SellTickerRequest({
        symbol,
        quantity,
        price
      });

      const response = await portfolioService.sellTicker(request);

      if (response.success) {
        // Refresh portfolio after successful sale
        await fetchPortfolio();
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Failed to sell ticker' };
      }
    } catch (error) {
      return { success: false, message: 'Failed to sell ticker' };
    }
  };

  const deleteTicker = async (symbol: string) => {
    try {
      const request = new StockyApi.DeleteTickerRequest({
        symbol
      });

      const response = await portfolioService.deleteTicker(request);

      if (response.success) {
        // Refresh portfolio after successful deletion
        await fetchPortfolio();
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Failed to delete ticker' };
      }
    } catch (error) {
      return { success: false, message: 'Failed to delete ticker' };
    }
  };

  const addFunds = async (amount: number) => {
    try {
      const request = new StockyApi.AddFundsRequest({
        amount
      });

      const response = await portfolioService.addFunds(request);

      if (response.success) {
        // Refresh portfolio after successful funds addition
        await fetchPortfolio();
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Failed to add funds' };
      }
    } catch (error) {
      return { success: false, message: 'Failed to add funds' };
    }
  };

  const subtractFunds = async (amount: number) => {
    try {
      const request = new StockyApi.SubtractFundsRequest({
        amount
      });

      const response = await portfolioService.subtractFunds(request);

      if (response.success) {
        // Refresh portfolio after successful funds subtraction
        await fetchPortfolio();
        return { success: true, message: response.message };
      } else {
        return { success: false, message: response.message || 'Failed to subtract funds' };
      }
    } catch (error) {
      return { success: false, message: 'Failed to subtract funds' };
    }
  };

  useEffect(() => {
    fetchPortfolio();
  }, []);

  return {
    ...portfolioState,
    fetchPortfolio,
    buyTicker,
    sellTicker,
    deleteTicker,
    addFunds,
    subtractFunds
  };
};