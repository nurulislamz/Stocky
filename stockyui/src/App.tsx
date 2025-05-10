import { BrowserRouter, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/Login/LoginPage';
import SignUpPage from './pages/SignUp/SignUpPage';
import HomePage from './pages/Home/HomePage';
import PortfolioPage from './pages/Portfolio/PortfolioPage';
import EtfCalculatorPage from './pages/EtfCalculator/EtfCalculatorPage';

const App = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/signup" element={<SignUpPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/portfolio" element={<PortfolioPage />} />
        <Route path="/etfcalculator" element={<EtfCalculatorPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;