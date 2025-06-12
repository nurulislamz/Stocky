import { StockyApi } from './api/stockyapi';

export class BaseService {
    protected api: StockyApi;

    constructor() {
        this.api = new StockyApi({
            baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7001'
        });
    }
}