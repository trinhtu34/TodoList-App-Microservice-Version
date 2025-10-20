import axios from 'axios';

const TODO_API_BASE_URL = import.meta.env.VITE_TODO_API_BASE_URL;

const todoApi = axios.create({
  baseURL: TODO_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

todoApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

todoApi.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('todoApi error:', error.response?.status, error.response?.data);
    return Promise.reject(error);
  }
);

export default todoApi;
