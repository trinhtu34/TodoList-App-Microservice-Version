import axios from 'axios';

const AUTH_API_BASE_URL = import.meta.env.VITE_AUTH_API_BASE_URL;

const authApi = axios.create({
  baseURL: AUTH_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

authApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

authApi.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const response = await axios.post(`${AUTH_API_BASE_URL}/auth/refresh`, {
            refreshToken: refreshToken
          });
          
          const newToken = response.data.accessToken;
          localStorage.setItem('accessToken', newToken);
          localStorage.setItem('idToken', response.data.idToken);
          
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return authApi.request(originalRequest);
        } catch (refreshError) {
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('idToken');
          window.location.href = '/login';
        }
      } else {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default authApi;
