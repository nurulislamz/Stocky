import { StockyApi } from './generated/stockyapi';
import axios, { AxiosError } from 'axios';

export class BaseService {
    protected api: StockyApi.StockyApi;

    constructor() {
        const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5115';

        const axiosInstance = axios.create();

        // Request interceptor
        axiosInstance.interceptors.request.use(
            (config) => {
                const token = localStorage.getItem('token');
                if (token) {
                    config.headers.Authorization = `Bearer ${token}`;
                }
                return config;
            },
            (error) => Promise.reject(error)
        );

        // Response interceptor for auth errors
        axiosInstance.interceptors.response.use(
            (response) => response,
            (error: AxiosError) => {
                if (error.response?.status === 401) {
                    // Token expired or invalid
                    localStorage.removeItem('token');
                    window.location.href = '/login';
                }
                return Promise.reject(error);
            }
        );

        this.api = new StockyApi.StockyApi(apiUrl, axiosInstance);
    }
}