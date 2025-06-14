import { createContext, useContext, useState, useEffect } from 'react';
import { StockyApi } from '../services/generated/stockyapi';
import { PortfolioService } from '../services/portfolio.service';

const PortfolioContext = createContext<{
  data: StockyApi.PortfolioData | null;
  refresh: () => Promise<void>;
} | null>(null);

export const PortfolioProvider = ({ children }: { children: React.ReactNode }) => {
  const [data, setData] = useState<StockyApi.PortfolioData | null>(null);
  const portfolioService = new PortfolioService();

  const refresh = async () => {
    const response = await portfolioService.getPortfolio({} as StockyApi.UserPortfolioRequest);
    if (response.success && response.data) {
      setData(response.data);
    }
  };

  useEffect(() => {
    refresh();
  }, []);

  return (
    <PortfolioContext.Provider value={{ data, refresh }}>
      {children}
    </PortfolioContext.Provider>
  );
};

export const usePortfolio = () => {
  const context = useContext(PortfolioContext);
  if (!context) throw new Error('usePortfolio must be used within PortfolioProvider');
  return context;
};