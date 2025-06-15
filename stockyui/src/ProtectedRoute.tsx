import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from './hooks/useAuth';
import { AuthService } from './services/auth.service';

export const ProtectedRoute = () => {
  const { isAuthenticated, logout } = useAuth();
  const authService = new AuthService();

  const forceLogout = () => {
    console.log('ProtectedRoute - Force clearing authentication state');
    authService.removeToken();
    logout();
  };

  const isReallyAuthenticated = authService.isAuthenticated();
  console.log('ProtectedRoute - Auth check:', {
    useAuthIsAuthenticated: isAuthenticated,
    authServiceIsAuthenticated: isReallyAuthenticated
  });

  if (!isReallyAuthenticated) {
    console.log('ProtectedRoute - Not authenticated, forcing logout and redirecting to login');
    forceLogout();
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
};