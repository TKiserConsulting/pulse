import {
    Injectable,
    Injector,
    FactoryProvider,
    APP_INITIALIZER,
} from '@angular/core';
import { Router } from '@angular/router';
import { Store, select } from '@ngrx/store';
import { LoggerService } from '@pulse/sdk';
import { ApiConfiguration } from '@app/api/api-configuration';
import { take } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import * as fromConfig from '../reducers/config.reducer';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class BootstrapService {
    constructor(
        private store: Store<any>,
        private configService: ConfigService,
        private translateService: TranslateService,
        private injector: Injector,
        private logger: LoggerService,
        apiConfiguration: ApiConfiguration
    ) {
        apiConfiguration.rootUrl = '';
    }

    public async bootstrap(): Promise<void> {
        let mode = null;
        try {
            this.logger.log('[boostrap service] init config');
            await this.configService.initialize();
        } catch (err) {
            this.logger.error(err);
            mode = 'bootstrap_failed#config';
        }

        try {
            this.logger.log('[boostrap service] init localizations');
            await this.initializeLocalization();
        } catch (err) {
            this.logger.error(err);
            mode = 'bootstrap_failed#l10n';
        }

        if (mode) {
            const router = this.injector.get(Router);
            router.navigate(['/error'], {
                queryParams: {
                    code: mode,
                    message: 'Error while application bootstrap',
                },
                skipLocationChange: true,
            });
        }
    }

    private async initializeLocalization(): Promise<void> {
        const config = await this.store
            .pipe(select(fromConfig.getConfig), take(1))
            .toPromise();

        if (config) {
            this.translateService.addLangs(config.uiLanguages);
            this.translateService.setDefaultLang(config.defaultUiLanguage);
        }

        // in case somebody says we have too slow loading
        // we can try use without await:
        // this.translateService.use(this.translateService.defaultLang);
        await this.translateService
            .use(this.translateService.defaultLang)
            .toPromise();
    }
}

export function BootstrapFactory(bootstrapService: BootstrapService) {
    return () => bootstrapService.bootstrap();
}

export const BOOTSTRAP_PROVIDER: FactoryProvider = {
    provide: APP_INITIALIZER,
    useFactory: BootstrapFactory,
    multi: true,
    deps: [BootstrapService],
};
