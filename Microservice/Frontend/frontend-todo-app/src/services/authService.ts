import api from '../config/api';

interface LoginResponse {
  success: boolean;
  accessToken?: string;
  idToken?: string;
  refreshToken?: string;
  expiresIn?: number;
  message?: string;
}

interface AuthResult {
  success: boolean;
  message?: string;
  data?: any;
}

class AuthService {
  private token: string | null;
  private refreshToken: string | null;

  constructor() {
    this.token = localStorage.getItem('accessToken');
    this.refreshToken = localStorage.getItem('refreshToken');
  }

  async login(email: string, password: string): Promise<AuthResult> {
    try {
      const response = await api.post<LoginResponse>('/auth/login', {
        email,
        password
      });

      if (response.data.success) {
        this.token = response.data.accessToken!;
        this.refreshToken = response.data.refreshToken!;
      
        localStorage.setItem('accessToken', this.token);
        localStorage.setItem('refreshToken', this.refreshToken);
        localStorage.setItem('idToken', response.data.idToken!);
        
        return { success: true, data: response.data };
      }
      
      return { success: false, message: response.data.message };
    } catch (error: any) {
      return { 
        success: false, 
        message: error.response?.data?.message || 'Đăng nhập thất bại' 
      };
    }
  }

  async register(email: string, password: string, name: string): Promise<AuthResult> {
    try {
      const response = await api.post('/auth/register', {
        email,
        password,
        name
      });
      
      return { success: true, data: response.data };
    } catch (error: any) {
      return { 
        success: false, 
        message: error.response?.data?.message || 'Đăng ký thất bại' 
      };
    }
  }

  async confirmSignUp(email: string, confirmationCode: string): Promise<AuthResult> {
    try {
      const response = await api.post('/auth/confirm', {
        email,
        confirmationCode
      });

      return { success: true, data: response.data };
    } catch (error: any) {
      return { 
        success: false, 
        message: error.response?.data?.message || 'Xác nhận thất bại' 
      };
    }
  }

  getUserFromToken() {
    // Always read from localStorage to handle refresh
    const token = this.getToken();
    console.log('getUserFromToken: Token exists?', !!token);
    
    if (!token) return null;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      console.log('getUserFromToken: Decoded payload:', payload);
      
      const userData = {
        username: payload.username || payload['cognito:username'],
        sub: payload.sub,
        name: payload.name || payload['cognito:username'] || payload.email,
        exp: payload.exp,
        groups: payload['cognito:groups'] || []
      };
      console.log('getUserFromToken: User data:', userData);
      return userData;
    } catch (error) {
      console.error('getUserFromToken: Error decoding token:', error);
      return null;
    }
  }

  isPremiumUser(): boolean {
    const userData = this.getUserFromToken();
    return userData?.groups?.includes('Premium-user') || false;
  }

  logout() {
    this.token = null;
    this.refreshToken = null;
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('idToken');
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }

  getToken(): string | null {
    // Always read from localStorage to handle refresh
    if (!this.token) {
      this.token = localStorage.getItem('accessToken');
    }
    return this.token;
  }
}

const authServiceInstance = new AuthService();
export default authServiceInstance;
