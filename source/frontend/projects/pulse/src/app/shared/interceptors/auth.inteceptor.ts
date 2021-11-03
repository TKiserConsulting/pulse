import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpErrorResponse,
} from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { select, Store } from '@ngrx/store';
import { Router } from '@angular/router';
import { LoggerService } from '@pulse/sdk';
import { AuthService } from '../services/auth.service';

import * as fromAuth from '../reducers/auth.reducer';
import { JwtAuthModel } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class JwtInterceptor implements HttpInterceptor {
    // todo [mi] review subject usage
    private refreshTokenSubject: BehaviorSubject<JwtAuthModel | null> = new BehaviorSubject<JwtAuthModel | null>(null);

    private isRefreshing = false;

    private jwtModel!: JwtAuthModel;

    constructor(
        private authService: AuthService,
        private store: Store<any>,
        private router: Router,
        private logger: LoggerService
    ) {
        this.store.pipe(select(fromAuth.getAuthJwt)).subscribe((jwt: any) => {
            this.jwtModel = jwt;
        });
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        const tokensSnapshot = this.jwtModel;

        const upgradedRequest = this.populateJwt(request, tokensSnapshot);

        return next.handle(upgradedRequest).pipe(
            catchError((error: any): Observable<HttpEvent<any>> => {
                if (
                    tokensSnapshot &&
                    tokensSnapshot.access_token &&
                    tokensSnapshot.refresh_token &&
                    error instanceof HttpErrorResponse &&
                    error.status === 401
                ) {
                    return this.handle401Error(request, next, tokensSnapshot, error);
                }
                return throwError(() => error);
            })
        );
    }

    private populateJwt(request: HttpRequest<any>, tokens: JwtAuthModel): HttpRequest<any> {
        if (tokens && tokens.access_token) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${tokens.access_token}`,
                },
            });
        }

        return request;
    }

    private handle401Error(
        request: HttpRequest<any>,
        next: HttpHandler,
        tokens: JwtAuthModel,
        originalError: any
    ): Observable<HttpEvent<any>> {
        if (!this.isRefreshing) {
            this.isRefreshing = true;

            this.refreshTokenSubject.next(null);

            return this.authService.refresh(tokens).pipe(
                catchError((innerError) => {
                    this.logger.log('Unhandled error during refresh', innerError);
                    this.authService.signout();
                    this.router.navigate(['/signin']);
                    // return this.handleOrThrow(request, next, null, innerError);
                    return throwError(() => innerError);
                }),
                switchMap((jwt: JwtAuthModel) => {
                    this.isRefreshing = false;
                    this.refreshTokenSubject.next(jwt);

                    return this.handleOrThrow(request, next, jwt, originalError);
                })
            );
        }
        return this.refreshTokenSubject.pipe(
            filter((m) => m != null),
            take(1),
            switchMap((jwt: any) => this.handleOrThrow(request, next, jwt, originalError))
        );
    }

    private handleOrThrow(
        request: HttpRequest<any>,
        next: HttpHandler,
        jwt: JwtAuthModel | null,
        error: any
    ): Observable<HttpEvent<any>> {
        if (jwt && jwt.refresh_token && jwt.access_token) {
            return next.handle(this.populateJwt(request, jwt));
        }

        return throwError(() => error);
    }
}
