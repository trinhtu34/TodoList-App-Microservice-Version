import React, { createContext, useContext, useState, useEffect } from 'react';
import type { ReactNode } from 'react';
import authService from '../services/authService';

interface User {
  username: string;
  sub: string;
  name: string;
  exp: number;
  groups: string[];
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<{ success: boolean; message?: string }>;
  register: (email: string, password: string, name: string) => Promise<{ success: boolean; message?: string }>;
  confirmSignUp: (email: string, code: string) => Promise<{ success: boolean; message?: string }>;
  logout: () => void;
  isAuthenticated: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Restore auth state from localStorage on mount/refresh
    console.log('AuthContext: Checking for existing token...');
    const token = authService.getToken();
    console.log('AuthContext: Token exists?', !!token);
    
    if (token) {
      const userData = authService.getUserFromToken();
      console.log('AuthContext: User data from token:', userData);
      
      if (userData) {
        // Check if token is expired
        const now = Math.floor(Date.now() / 1000);
        console.log('AuthContext: Token exp:', userData.exp, 'Now:', now, 'Valid:', userData.exp > now);
        
        if (userData.exp > now) {
          setUser(userData);
          setIsAuthenticated(true);
          console.log('AuthContext: User restored successfully');
        } else {
          // Token expired, clear it
          console.log('AuthContext: Token expired, logging out');
          authService.logout();
        }
      } else {
        console.log('AuthContext: Failed to decode user data');
      }
    } else {
      console.log('AuthContext: No token found');
    }
    setLoading(false);
  }, []);

  const login = async (email: string, password: string) => {
    const result = await authService.login(email, password);
    
    if (result.success) {
      const userData = authService.getUserFromToken();
      setUser(userData);
      setIsAuthenticated(true);
    }
    
    return result;
  };

  const register = async (email: string, password: string, name: string) => {
    return await authService.register(email, password, name);
  };

  const confirmSignUp = async (email: string, code: string) => {
    return await authService.confirmSignUp(email, code);
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    setIsAuthenticated(false);
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <AuthContext.Provider value={{ user, login, register, confirmSignUp, logout, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
