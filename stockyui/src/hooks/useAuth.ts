import { useState, useEffect } from 'react';
import { AuthService } from '../services/auth.service';

interface AuthState {
  isAuthenticated: boolean;
  token: string | null;
}

export const useAuth = () => {
  const authService = new AuthService();
  const [authState, setAuthState] = useState<AuthState>(() => {
    // Initialize from AuthService
    const token = authService.getToken();
    return {
      isAuthenticated: authService.isAuthenticated(),
      token: token
    };
  });

  useEffect(() => {
    // Check authentication status periodically
    const checkAuth = () => {
      const isReallyAuthenticated = authService.isAuthenticated();
      const token = authService.getToken();

      setAuthState({
        isAuthenticated: isReallyAuthenticated,
        token: token
      });
    };

    // Check immediately
    checkAuth();

    // Set up interval to check more frequently
    const intervalId = setInterval(checkAuth, 60000); // Check every 30 seconds

    // Also check on window focus
    const handleFocus = () => checkAuth();
    window.addEventListener('focus', handleFocus);

    return () => {
      clearInterval(intervalId);
      window.removeEventListener('focus', handleFocus);
    };
  }, []);

  const login = async (token: string) => {
    authService.setToken(token);
    setAuthState({
      isAuthenticated: true,
      token
    });
  };

  const logout = () => {
    authService.removeToken();
    setAuthState({
      isAuthenticated: false,
      token: null
    });
  };

  return {
    isAuthenticated: authState.isAuthenticated,
    token: authState.token,
    login,
    logout
  };
};