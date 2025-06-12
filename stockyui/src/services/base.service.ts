import { StockyApi } from './generated/stockyapi';

export class BaseService {
    protected api: StockyApi.StockyApi;

    constructor() {
        this.api = new StockyApi.StockyApi(process.env.REACT_APP_API_URL || 'https://localhost:7001');
    }
}