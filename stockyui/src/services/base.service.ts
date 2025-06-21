import { StockyApi } from './generated/stockyapi';

export class BaseService {
    protected api: StockyApi.StockyApi;

    constructor() {
        const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5115';
        console.log('BaseService - API URL:', apiUrl);
        this.api = new StockyApi.StockyApi(apiUrl);
    }
}