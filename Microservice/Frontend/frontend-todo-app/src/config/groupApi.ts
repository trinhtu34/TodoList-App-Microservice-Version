import axios from 'axios';

const GROUP_API_BASE_URL = import.meta.env.VITE_GROUP_API_BASE_URL;

const groupApi = axios.create({
  baseURL: GROUP_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

groupApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

groupApi.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('groupApi error:', error.response?.status, error.response?.data);
    return Promise.reject(error);
  }
);

export default groupApi;
