import { AclService } from '@app/api/services';
import { forkJoin, from, Observable, of } from 'rxjs';
import { catchError, map, shareReplay, tap } from 'rxjs/operators';
import {
    GrantAvailabilityData,
    GrantDescriptor,
    GrantResult,
} from '@app/api/models';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class GrantService {
    private grantRequests: {
        [k: string]: Promise<GrantAvailabilityData | undefined>;
    } = {};

    constructor(
        private aclService: AclService,
        private authService: AuthService
    ) {
        this.authService.identity$.subscribe(() => {
            this.grantRequests = {}; // reset
        });
    }

    public async hasGrants(
        values: GrantDescriptor[] | string[] | string
    ): Promise<boolean> {
        const r = await this.validateGrants(values);
        return r.allow || false;
    }

    public async validateGrants(
        values: GrantDescriptor[] | string[] | string
    ): Promise<GrantResult> {
        const grants = this.buildGrants(values);
        return this.retrieveGrants(grants);
    }

    private async retrieveGrants(
        grants: GrantDescriptor[]
    ): Promise<GrantResult> {
        // find unknown grants
        const grantsToCheck = grants.filter((grant) => {
            const key = `${grant.type}:${grant.action}`;
            return !this.grantRequests[key];
        });

        // prepeare new grants check
        let mainRequest: Observable<GrantResult> | null = null;
        if (grantsToCheck.length > 0) {
            let obs: Observable<GrantResult>;
            if (environment.features.authorization) {
                obs =
                    grantsToCheck.length > 1
                        ? this.aclService.checkMany({
                              body: grantsToCheck,
                          })
                        : this.aclService.checkSingle({
                              grantType: grantsToCheck[0].type || '',
                              grantAction: grantsToCheck[0].action || '',
                          });

                obs = obs.pipe(
                    catchError(() =>
                        of({
                            grantsAvailability: grantsToCheck.map((grant) => ({
                                grant: { ...grant },
                                granted: undefined,
                            })),
                            allow: undefined,
                        } as GrantResult)
                    ),
                    shareReplay(1)
                );
            } else {
                obs = of({
                    grantsAvailability: grantsToCheck.map((grant) => ({
                        grant: { ...grant },
                        granted: true,
                    })),
                    allow: true,
                } as GrantResult);
            }

            mainRequest = obs;
        }

        const requests: Observable<GrantAvailabilityData | undefined>[] =
            grants.map((grant) => {
                const key = `${grant.type}:${grant.action}`;
                let result: Promise<GrantAvailabilityData | undefined> =
                    this.grantRequests[key];
                if (!result && mainRequest) {
                    result = mainRequest
                        .pipe(
                            map((r) =>
                                r.grantsAvailability?.find(
                                    (gad) =>
                                        (!!grant.identifier &&
                                            gad.grant?.identifier ===
                                                grant.identifier) ||
                                        (gad.grant?.type === grant.type &&
                                            gad.grant?.action === grant.action)
                                )
                            ),
                            tap((gad) => {
                                if (gad?.granted === undefined) {
                                    delete this.grantRequests[key];
                                }
                            })
                        )
                        .toPromise();

                    this.grantRequests[key] = result;
                }

                return from(result);
            });

        const grantsAvailability = await forkJoin(requests).toPromise();
        const res: GrantResult = {
            allow: !grantsAvailability?.some((a) => !a?.granted),
            grantsAvailability: grantsAvailability as any,
        };

        return res;
    }

    private buildGrants(
        values: GrantDescriptor[] | string[] | string
    ): GrantDescriptor[] {
        let grants: GrantDescriptor[] = [];
        if (values instanceof Array) {
            if (values.length > 0 && typeof values[0] === 'string') {
                grants = (values as string[]).map((v) =>
                    this.buildGrantFromString(v)
                );
            } else {
                grants = values as GrantDescriptor[];
            }
        } else if (typeof values === 'string') {
            grants = [this.buildGrantFromString(values)];
        } else {
            throw new Error('Unsupported type');
        }

        return grants;
    }

    private buildGrantFromString(value: string): GrantDescriptor {
        const parts = value.split(':');
        if (parts.length !== 2) {
            throw new Error('Invalid grant reference');
        }

        const type = parts[0];
        const action = parts[1];
        return {
            type,
            action,
        };
    }
}
