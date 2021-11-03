import { Injectable } from '@angular/core';
import {
    RegisterRequestDto,
    SigninRequestDto,
    TokenDto,
    UserDetailsDto,
} from '@app/api/models';
import { AuthenticationService } from '@app/api/services';
import { select, Store } from '@ngrx/store';

import JWT from 'jwt-decode';

import * as fromAuth from '@app/shared/reducers/auth.reducer';
import { map, switchMap, take } from 'rxjs/operators';
import { lastValueFrom, Observable } from 'rxjs';
import { Identity, JwtAuthModel, ProfileClaims } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
    public identity$: Observable<Identity>;

    public isAuthenticated$: Observable<boolean>;

    constructor(
        private authenticationService: AuthenticationService,
        private store: Store<any>
    ) {
        this.identity$ = this.store.pipe(select(fromAuth.getAuthIdentity));
        this.isAuthenticated$ = this.store.pipe(
            select(fromAuth.isAuthenticated)
        );
    }

    public async getUserId(): Promise<string | undefined | null> {
        return this.store
            .pipe(
                select(fromAuth.getAuthIdentity),
                map((i) => i?.userId),
                take(1)
            )
            .toPromise();
    }

    public async getAccessToken(): Promise<string | undefined | null> {
        return this.store
            .pipe(
                select(fromAuth.getAuthJwt),
                map((i) => i?.access_token),
                take(1)
            )
            .toPromise();
    }

    public async signin(dto: SigninRequestDto): Promise<JwtAuthModel> {
        const result = await lastValueFrom(
            this.authenticationService.signin({ body: dto })
        );

        return this.storeIdentity(result.user, result.tokenInfo);
    }

    public signout(): void {
        // can be run in parallel
        this.store
            .select(fromAuth.getAuthJwt)
            .pipe(
                take(1),
                switchMap((jwt) =>
                    this.authenticationService.signout({
                        body: { refreshToken: jwt?.refresh_token },
                    })
                )
            )
            .subscribe
            // not care about result
            ();

        this.store.dispatch(fromAuth.clearAuth());
    }

    public refresh(jwt: JwtAuthModel): Observable<JwtAuthModel> {
        const dto = {
            tokenInfo: {
                accessToken: jwt.access_token,
                refreshToken: jwt.refresh_token,
            },
        };

        return this.authenticationService.refresh({ body: dto }).pipe(
            map((result) => {
                return this.storeTokens(result.tokenInfo);
            })
        );
    }

    public async register(dto: RegisterRequestDto): Promise<JwtAuthModel> {
        const result = await lastValueFrom(
            this.authenticationService.registerAccount({ body: dto })
        );

        return this.storeIdentity(result.user, result.tokenInfo);
    }

    private storeIdentity(
        user: UserDetailsDto,
        tokenInfo: TokenDto
    ): JwtAuthModel {
        const token = JWT(tokenInfo.accessToken) as any;

        const jwt = {
            access_token: tokenInfo.accessToken,
            refresh_token: tokenInfo.refreshToken,
            expires_in: token.exp,
        };

        const identity = {
            userId: token[ProfileClaims.userId],
            userName: token[ProfileClaims.userName],
            email: token[ProfileClaims.userEmail],
            userRole: token[ProfileClaims.userRole],
            isAuthenticated: true,
            firstName: user.firstName,
            lastName: user.lastName,
            school: user.school,
            state: user.state,
            subject: user.subject,
        };

        this.store.dispatch(
            fromAuth.setAuth({
                jwt,
                identity,
            })
        );

        return jwt;
    }

    private storeTokens(tokenInfo: TokenDto): JwtAuthModel {
        const token = JWT(tokenInfo.accessToken) as any;

        const jwt = {
            access_token: tokenInfo.accessToken,
            refresh_token: tokenInfo.refreshToken,
            expires_in: token.exp,
        };

        this.store.dispatch(
            fromAuth.setTokens({
                jwt,
            })
        );

        return jwt;
    }
}
