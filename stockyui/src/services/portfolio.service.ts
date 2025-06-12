import { BaseService } from './base.service';
import { UserPortfolioRequest, BuyTickerRequest, SellTickerRequest, PortfolioResponse, TickerInfo } from './generated/stockyapi';

export class PortfolioService extends BaseService {
  async getPortfolio(request: UserPortfolioRequest) {
    return this.api.getPortfolio(request);
  }

  async buyTicker(request: BuyTickerRequest) {
    return this.api.buyTicker(request);
  }

  async sellTicker(request: SellTickerRequest) {
    return this.api.sellTicker(request);
  }

  calculateTotalValue(portfolio: PortfolioResponse): number {
    return portfolio.tickers.reduce((total, ticker) => {
      return total + (ticker.currentPrice * ticker.quantity);
    }, 0);
  }

  calculateProfitLoss(portfolio: PortfolioResponse): number {
    return portfolio.tickers.reduce((total, ticker) => {
      const currentValue = ticker.currentPrice * ticker.quantity;
      const purchaseValue = ticker.averagePrice * ticker.quantity;
      return total + (currentValue - purchaseValue);
    }, 0);
  }

  getTopPerformers(portfolio: PortfolioResponse, limit: number = 5): TickerInfo[] {
    return [...portfolio.tickers]
      .sort((a, b) => {
        const aReturn = (a.currentPrice - a.averagePrice) / a.averagePrice;
        const bReturn = (b.currentPrice - b.averagePrice) / b.averagePrice;
        return bReturn - aReturn;
      })
      .slice(0, limit);
  }

  getWorstPerformers(portfolio: PortfolioResponse, limit: number = 5): TickerInfo[] {
    return [...portfolio.tickers]
      .sort((a, b) => {
        const aReturn = (a.currentPrice - a.averagePrice) / a.averagePrice;
        const bReturn = (b.currentPrice - b.averagePrice) / b.averagePrice;
        return aReturn - bReturn;
      })
      .slice(0, limit);
  }

  calculatePortfolioAllocation(portfolio: PortfolioResponse): Map<string, number> {
    const totalValue = this.calculateTotalValue(portfolio);
    const allocation = new Map<string, number>();

    portfolio.tickers.forEach(ticker => {
      const tickerValue = ticker.currentPrice * ticker.quantity;
      const percentage = (tickerValue / totalValue) * 100;
      allocation.set(ticker.symbol, percentage);
    });

    return allocation;
  }
}