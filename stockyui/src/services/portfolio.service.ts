import { BaseService } from './base.service';
import { StockyApi } from './generated/stockyapi';

export class PortfolioService extends BaseService {
  // API Methods
  async getPortfolio() {
    try {
      return await this.api.portfolio();
    } catch (error) {
      console.error('Error fetching portfolio:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to fetch portfolio'
      } as StockyApi.UserPortfolioResponse;
    }
  }

  async buyTicker(request: StockyApi.BuyTickerRequest) {
    try {
      return await this.api.buy(request);
    } catch (error) {
      console.error('Error buying ticker:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to buy ticker'
      } as StockyApi.BuyTickerResponse;
    }
  }

  async sellTicker(request: StockyApi.SellTickerRequest) {
    try {
      return await this.api.sell(request);
    } catch (error) {
      console.error('Error selling ticker:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to sell ticker'
      } as StockyApi.SellTickerResponse;
    }
  }

  async addFunds(request: StockyApi.AddFundsRequest) {
    try {
      return await this.api.addfunds(request);
    } catch (error) {
      console.error('Error adding funds:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to add funds'
      } as StockyApi.AddFundsResponse;
    }
  }

  async subtractFunds(request: StockyApi.SubtractFundsRequest) {
    try {
      return await this.api.subtractfunds(request);
    } catch (error) {
      console.error('Error subtracting funds:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to subtract funds'
      } as StockyApi.SubtractFundsResponse;
    }
  }

  async setFunds(request: StockyApi.SetFundsRequest) {
    try {
      return await this.api.setfunds(request);
    } catch (error) {
      console.error('Error setting funds:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to set funds'
      } as StockyApi.SetFundsResponse;
    }
  }

  async deleteTicker(request: StockyApi.DeleteTickerRequest) {
    try {
      return await this.api.delete(request);
    } catch (error) {
      console.error('Error deleting ticker:', error);
      return {
        success: false,
        statusCode: 500,
        message: 'Failed to delete ticker'
      } as StockyApi.DeleteTickerResponse;
    }
  }

  // Portfolio Analysis
  calculateTotalValue(portfolio: StockyApi.PortfolioData): number {
    return portfolio.totalValue || 0;
  }

  calculateProfitLoss(portfolio: StockyApi.PortfolioData): number {
    return portfolio.items?.reduce((total, item) => {
      return total + (item.profitLoss || 0);
    }, 0) || 0;
  }

  getTopPerformers(portfolio: StockyApi.PortfolioData, limit: number = 5): StockyApi.PortfolioItem[] {
    return [...(portfolio.items || [])]
      .sort((a, b) => (b.profitLossPercentage || 0) - (a.profitLossPercentage || 0))
      .slice(0, limit);
  }

  getWorstPerformers(portfolio: StockyApi.PortfolioData, limit: number = 5): StockyApi.PortfolioItem[] {
    return [...(portfolio.items || [])]
      .sort((a, b) => (a.profitLossPercentage || 0) - (b.profitLossPercentage || 0))
      .slice(0, limit);
  }

  calculateAllocation(portfolio: StockyApi.PortfolioData): Map<string, number> {
    const totalValue = this.calculateTotalValue(portfolio);
    const allocation = new Map<string, number>();

    portfolio.items?.forEach(item => {
      if (item.symbol && item.totalValue) {
        const percentage = (item.totalValue / totalValue) * 100;
        allocation.set(item.symbol, percentage);
      }
    });

    return allocation;
  }
}