import { Pipe, PipeTransform } from '@angular/core';
import { LoggerService } from '@pulse/sdk';
import { GrantService } from '../services/grant.service';

@Pipe({ name: 'appGranted' })
export class GrantedPipe implements PipeTransform {
    public constructor(
        private grantService: GrantService,
        private loggerService: LoggerService
    ) {}

    public async transform(value: string): Promise<boolean> {
        let result: boolean;
        try {
            result = await this.grantService.hasGrants(value);
        } catch (e) {
            result = false;
            this.loggerService.error('Error while checking grants', e);
        }

        return result;
    }
}
