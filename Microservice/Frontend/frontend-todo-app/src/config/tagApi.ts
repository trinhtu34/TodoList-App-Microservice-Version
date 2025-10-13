import axios from 'axios';

const TAG_API_BASE_URL = import.meta.env.VITE_TAG_API_BASE_URL;

const tagApi = axios.create({
  baseURL: TAG_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

tagApi.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  console.log('tagApi: Sending request with token:', !!token);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
    console.log('tagApi: Authorization header set');
  }
  return config;
});

tagApi.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('tagApi error:', error.response?.status, error.response?.data);
    return Promise.reject(error);
  }
);

export default tagApi;
