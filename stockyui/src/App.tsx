import { BrowserRouter, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/Login/LoginPage';
import SignUpPage from './pages/SignUp/SignUpPage';
import HomePage from './pages/Home/HomePage';
import SearchPage from "./pages/Search/SearchPage";
import PortfolioPage from './pages/Portfolio/PortfolioPage';
import EtfCalculatorPage from './pages/EtfCalculator/EtfCalculatorPage';
import WatchListPage from './pages/WatchList/WatchListPage';
import MarketDataPage from './pages/MarketData/MarketDataPage';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignUpPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/search" element={<SearchPage />} />
        <Route path="/portfolio" element={<PortfolioPage />} />
        <Route path="/etfcalculator" element={<EtfCalculatorPage />} />
        <Route path="/watchlist" element={<WatchListPage />} />
        <Route path="/marketdata" element={<MarketDataPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;