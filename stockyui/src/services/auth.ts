// Token mnanagement and authentication service
interface DecodedToken {
  issuer: string,
  audience: string,
  claims: string,
  expires: number,
  signingCredentials: string
}


export const AuthService = {
  decodeToken(token: string): DecodedToken | null {
    try {
      const base64Payload = token.split('.')[1];
      const payload = atob(base64Payload);
      return JSON.parse(payload);
    }
    catch {
      return null;
    }
  },

  isTokenExpired(token: string): boolean {
    const decoded = this.decodeToken(token);
    if (!decoded) return true;

    const currentTime = Date.now() / 1000;
    return decoded.expires < currentTime;
  },

  getToken(): string | null {
    const token = localStorage.getItem("token");

    if (!token) return null;

    if (this.isTokenExpired(token)) {
      this.removeToken();
      return null;
    }
    return token;
  },

  setToken: (token: string): void => {
    localStorage.setItem("token", token)
  },

  removeToken: (): void => {
    localStorage.removeItem("token")
  },

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.isTokenExpired(token);
  },
}