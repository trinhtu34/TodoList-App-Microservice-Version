import React, { createContext, useContext, useState, useEffect, useMemo } from 'react';
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
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Memoize isAuthenticated to prevent unnecessary re-renders
  const isAuthenticated = useMemo(() => !!user, [user]);

  useEffect(() => {
    // Restore auth state from localStorage on mount/refresh
    const initializeAuth = async () => {
      try {
        const token = authService.getToken();
        
        if (token) {
          const userData = authService.getUserFromToken();
          
          if (userData) {
            // Check if token is expired
            const now = Math.floor(Date.now() / 1000);
            
            if (userData.exp > now) {
              setUser(userData);
              // Only log in development
              if (import.meta.env.DEV) {
                console.log('AuthContext: User restored successfully');
              }
            } else {
              // Token expired, clear it
              authService.logout();
              if (import.meta.env.DEV) {
                console.log('AuthContext: Token expired, logging out');
              }
            }
          }
        }
      } catch (error) {
        if (import.meta.env.DEV) {
          console.error('AuthContext: Error initializing auth:', error);
        }
        authService.logout();
      } finally {
        setIsLoading(false);
      }
    };

    initializeAuth();
  }, []);

  // Memoize functions to prevent unnecessary re-renders
  const login = useMemo(() => async (email: string, password: string) => {
    const result = await authService.login(email, password);
    
    if (result.success) {
      const userData = authService.getUserFromToken();
      setUser(userData);
    }
    
    return result;
  }, []);

  const register = useMemo(() => async (email: string, password: string, name: string) => {
    return await authService.register(email, password, name);
  }, []);

  const confirmSignUp = useMemo(() => async (email: string, code: string) => {
    return await authService.confirmSignUp(email, code);
  }, []);

  const logout = useMemo(() => () => {
    authService.logout();
    setUser(null);
  }, []);

  // Memoize context value to prevent unnecessary re-renders
  const contextValue = useMemo(() => ({
    user,
    login,
    register,
    confirmSignUp,
    logout,
    isAuthenticated,
    isLoading,
  }), [user, login, register, confirmSignUp, logout, isAuthenticated, isLoading]);

  // Show loading spinner during initialization
  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <AuthContext.Provider value={contextValue}>
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