import { useState, useEffect } from 'react';
import { AuthService } from '../services/auth.service';

interface UserInfo {
  id: string;
  email: string;
  firstName: string;
  surname: string;
  role: string;
}

interface AuthState {
  isAuthenticated: boolean;
  token: string | null;
  user: UserInfo | null;
}

export const useAuth = () => {
  const authService = new AuthService();
  const [authState, setAuthState] = useState<AuthState>(() => {
    // Initialize from AuthService
    const token = authService.getToken();
    const user = authService.getUserInfo();
    return {
      isAuthenticated: authService.isAuthenticated(),
      token: token,
      user: user
    };
  });

  useEffect(() => {
    // Check authentication status periodically
    const checkAuth = () => {
      const isReallyAuthenticated = authService.isAuthenticated();
      const token = authService.getToken();
      const user = authService.getUserInfo();

      setAuthState({
        isAuthenticated: isReallyAuthenticated,
        token: token,
        user: user
      });
    };

    // Check immediately
    checkAuth();

    // Set up interval to check more frequently
    const intervalId = setInterval(checkAuth, 60000); // Check every 60 seconds

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
    const user = authService.getUserInfo();
    setAuthState({
      isAuthenticated: true,
      token,
      user
    });
  };

  const logout = () => {
    authService.removeToken();
    setAuthState({
      isAuthenticated: false,
      token: null,
      user: null
    });
  };

  return {
    isAuthenticated: authState.isAuthenticated,
    token: authState.token,
    user: authState.user,
    login,
    logout
  };
};