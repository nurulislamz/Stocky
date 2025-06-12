import { BaseService } from './base.service';
import { LoginRequest, RegisterRequest } from './api/stockyapi';

export class AuthService extends BaseService {
    async login(request: LoginRequest) {
        return this.api.login(request);
    }

    async register(request: RegisterRequest) {
        return this.api.register(request);
    }
}