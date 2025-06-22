import { BaseService } from "./base.service";
import { StockyApi } from "./generated/stockyapi";

interface DecodedToken {
  issuer: string;
  audience: string;
  claims: string;
  expires: number;
  signingCredentials: string;
}

interface UserInfo {
  id: string;
  email: string;
  firstName: string;
  surname: string;
  role: string;
}

export class AuthService extends BaseService {
  // API Methods
  async login(request: StockyApi.LoginRequest) {
    const response = await this.api.login(request);
    if (response.data?.token) {
      this.setToken(response.data.token);
    }
    return response;
  }

  async register(request: StockyApi.RegisterRequest) {
    return this.api.register(request);
  }

  // Token Management
  private decodeToken(token: string): DecodedToken | null {
    try {
      const base64Payload = token.split(".")[1];
      const payload = atob(base64Payload);
      return JSON.parse(payload);
    } catch {
      return null;
    }
  }

  private isTokenExpired(token: string): boolean {
    const decoded = this.decodeToken(token);
    if (!decoded) return true;
    const currentTime = Date.now() / 1000;
    return decoded.expires < currentTime;
  }

  getUserInfo(): UserInfo | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const base64Payload = token.split(".")[1];
      const payload = atob(base64Payload);
      const decoded = JSON.parse(payload);

      return {
        id: decoded.userId,
        email: decoded.email,
        firstName: decoded.firstName,
        surname: decoded.surname,
        role: decoded.role
      };
    } catch {
      return null;
    }
  }

  getToken(): string | null {
    const token = localStorage.getItem("token");
    if (!token) return null;
    if (this.isTokenExpired(token)) {
      this.removeToken();
      return null;
    }
    return token;
  }

  setToken(token: string): void {
    localStorage.setItem("token", token);
  }

  removeToken(): void {
    localStorage.removeItem("token");
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  }
}
