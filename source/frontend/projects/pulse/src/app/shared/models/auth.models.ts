import { UserRole } from '@app/api/models';

export interface JwtAuthModel {
    access_token: string;
    expires_in: number;
    refresh_token: string;
}

export interface Identity {
    userId: string | null;
    userName: string;
    email: string | null;
    userRole: UserRole | null;
    isAuthenticated: boolean;
    firstName?: null | string;
    lastName?: null | string;
    school?: null | string;
    state?: null | string;
    subject?: null | string;
    sessionId?: null | string;
}

export enum ProfileClaims {
    userId = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
    userName = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
    userEmail = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
    userRole = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
}
