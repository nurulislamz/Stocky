// Basic user information
export interface UserModel {
  id: number;
  email: string;
  firstName: string;
  surname: string;
  password: string;
  createdAt: Date;
  lastLogin: Date;
  isActive: boolean;
  role: UserRole;
}

// User roles and permissions
export enum UserRole {
  USER = 'user',
  PREMIUM = 'premium',
  ADMIN = 'admin'
}

// User preferences and settings
export interface UserPreferences {
  userId: number;
  theme: 'light' | 'dark' | 'system';
  notifications: {
    email: boolean;
    push: boolean;
    priceAlerts: boolean;
    newsAlerts: boolean;
  };
  defaultCurrency: string;
  language: string;
  timezone: string;
}

// User portfolio
export interface UserPortfolio {
  userId: number;
  totalValue: number;
  cashBalance: number;
  investedAmount: number;
  performance: {
    daily: number;
    weekly: number;
    monthly: number;
    yearly: number;
  };
  lastUpdated: Date;
}

// User watchlist
export interface WatchlistItem {
  id: number;
  userId: number;
  symbol: string;
  addedAt: Date;
  notes?: string;
  targetPrice?: number;
  stopLoss?: number;
}

// User transactions
export interface Transaction {
  id: number;
  userId: number;
  symbol: string;
  type: 'BUY' | 'SELL';
  quantity: number;
  price: number;
  totalAmount: number;
  timestamp: Date;
  status: 'PENDING' | 'COMPLETED' | 'FAILED';
  orderType: 'MARKET' | 'LIMIT' | 'STOP';
  limitPrice?: number;
}

// User alerts
export interface PriceAlert {
  id: number;
  userId: number;
  symbol: string;
  targetPrice: number;
  condition: 'ABOVE' | 'BELOW';
  isTriggered: boolean;
  createdAt: Date;
  triggeredAt?: Date;
}

// User notifications
export interface UserNotification {
  id: number;
  userId: number;
  type: 'PRICE_ALERT' | 'NEWS' | 'SYSTEM' | 'PORTFOLIO';
  title: string;
  message: string;
  isRead: boolean;
  createdAt: Date;
  readAt?: Date;
}

// User session
export interface UserSession {
  id: number;
  userId: number;
  token: string;
  deviceInfo: {
    deviceType: string;
    browser: string;
    os: string;
    ipAddress: string;
  };
  lastActivity: Date;
  expiresAt: Date;
}

// User activity log
export interface UserActivity {
  id: number;
  userId: number;
  action: string;
  details: Record<string, any>;
  timestamp: Date;
  ipAddress: string;
}

// User verification
export interface UserVerification {
  userId: number;
  emailVerified: boolean;
  phoneVerified: boolean;
  kycStatus: 'PENDING' | 'VERIFIED' | 'REJECTED';
  documents: {
    type: string;
    status: 'PENDING' | 'VERIFIED' | 'REJECTED';
    uploadedAt: Date;
    verifiedAt?: Date;
  }[];
}