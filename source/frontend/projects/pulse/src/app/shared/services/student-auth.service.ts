import { Injectable } from '@angular/core';
import { JoinSessionRequestDto, TokenDto, UserRole } from '@app/api/models';
import { StudentsService } from '@app/api/services';
import { Store, select } from '@ngrx/store';
import { Observable, map, lastValueFrom } from 'rxjs';
import { JwtAuthModel } from '../models/auth.models';
import * as fromAuth from '@app/shared/reducers/auth.reducer';
import JWT from 'jwt-decode';

@Injectable({
    providedIn: 'root',
})
export class StudentAuthService {
    public isAuthenticated$: Observable<boolean>;

    constructor(
        private studentsService: StudentsService,
        private store: Store<any>
    ) {
        this.isAuthenticated$ = this.store.pipe(
            select(fromAuth.getAuthJwt),
            map((jwt) => !!jwt && !!jwt.access_token)
        );
    }

    public async signin(dto: JoinSessionRequestDto): Promise<JwtAuthModel> {
        const result = await lastValueFrom(
            this.studentsService.signin({ body: dto })
        );

        return this.storeTokens(
            result.tokenInfo,
            result.sessionStudentId,
            result.sessionId
        );
    }

    private storeTokens(
        tokenInfo: TokenDto,
        sessionStudentId: string,
        sessionId: string
    ): JwtAuthModel {
        const token = JWT(tokenInfo.accessToken) as any;

        const jwt = {
            access_token: tokenInfo.accessToken,
            refresh_token: tokenInfo.refreshToken,
            expires_in: token.exp,
        };

        const identity = {
            userId: sessionStudentId,
            userName: sessionStudentId,
            email: null,
            userRole: UserRole.Student,
            isAuthenticated: true,
            sessionId: sessionId,
        };

        this.store.dispatch(
            fromAuth.setAuth({
                jwt,
                identity,
            })
        );

        return jwt;
    }
}
