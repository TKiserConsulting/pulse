import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { Injectable } from '@angular/core';
import { select, Store } from '@ngrx/store';
import { LoggerService } from '@pulse/sdk';
import { Identity } from '../models/auth.models';

import * as fromAuth from '../reducers/auth.reducer';
import { GrantService } from '../services/grant.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    isAuthenticated: boolean = false;

    identity: Identity | null = null;

    constructor(
        private store: Store<any>,
        private router: Router,
        private logger: LoggerService,
        private grantService: GrantService
    ) {
        this.store
            .pipe(select(fromAuth.getAuthIdentity))
            .subscribe((identity) => {
                this.logger.log('[guard] identity:', identity);
                this.identity = identity;
                this.isAuthenticated = identity?.isAuthenticated || false;
            });
    }

    async canActivate(route: ActivatedRouteSnapshot): Promise<boolean> {
        this.logger.log(
            `[guard] ${
                this.isAuthenticated
                    ? 'user authenticated'
                    : 'user not authenticated'
            }`
        );

        if (this.isAuthenticated) {
            const hasAccess = await this.demandAccessRights(route);
            if (!hasAccess) {
                const { accessDeniedRoute } = route.data;
                if (accessDeniedRoute) {
                    return this.router.navigate(accessDeniedRoute);
                }
            }
            return hasAccess;
        }

        const { signinRoute } = route.data;
        return this.router.navigate(signinRoute ?? ['signin']);
    }

    private async demandAccessRights(
        route: ActivatedRouteSnapshot
    ): Promise<boolean> {
        const { grantType, userRole } = route.data;
        let hasAccess = true;
        if (grantType) {
            this.logger.log('[guard] has grants', grantType);
            hasAccess = await this.grantService.hasGrants(grantType);
            if (!hasAccess) {
                this.router.navigate(['error403']);
            }
        } else if (userRole) {
            hasAccess = this.identity?.userRole === userRole;
            this.logger.log(
                `[guard] role '${userRole}' granted '${hasAccess}'`,
                this.identity
            );
        }

        return hasAccess;
    }
}
