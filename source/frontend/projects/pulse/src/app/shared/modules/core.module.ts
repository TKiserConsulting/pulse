import { NgModule, Optional, SkipSelf, Injector } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { StoreModule } from '@ngrx/store';
import { TranslateModule } from '@ngx-translate/core';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { environment } from '@env/environment';
import { ROOT_REDUCERS, metaReducers } from '@app/shared/reducers';
import { LoggerModule } from 'ngx-logger';
import {
    HTTP_LOCALIZATION_INTERCEPTOR_PROVIDER,
    HTTP_ERROR_INTERCEPTOR_PROVIDER,
    HTTP_CONFIG_INTERCEPTOR_PROVIDER,
    ANONYMOUS_TRANSLATE_LOADER_PROVIDER,
    LOGGER_CONFIG_PROVIDER,
} from '@pulse/sdk';
import { BOOTSTRAP_PROVIDER } from '../services/bootstrap.service';
import { setStaticInjector } from '../services/inject.service';
import { JwtInterceptor } from '../interceptors/auth.inteceptor';

@NgModule({
    imports: [
        HttpClientModule,

        // ngrx
        StoreModule.forRoot(ROOT_REDUCERS, {
            metaReducers,
            runtimeChecks: {
                strictActionWithinNgZone: environment.development,
                strictStateSerializability: environment.development,
                strictActionSerializability: environment.development,
            },
        }),
        StoreDevtoolsModule.instrument({
            name: 'Pulse Portal',
            logOnly: environment.development,
        }),

        LoggerModule.forRoot(null),

        TranslateModule.forRoot({
            loader: ANONYMOUS_TRANSLATE_LOADER_PROVIDER,
        }),
    ],
    providers: [
        BOOTSTRAP_PROVIDER,
        HTTP_ERROR_INTERCEPTOR_PROVIDER,
        HTTP_LOCALIZATION_INTERCEPTOR_PROVIDER,
        HTTP_CONFIG_INTERCEPTOR_PROVIDER,
        { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
        LOGGER_CONFIG_PROVIDER(environment.development),
    ],
})
export class CoreModule {
    constructor(
        @Optional()
        @SkipSelf()
        parentModule: CoreModule,
        private injector: Injector
    ) {
        if (parentModule) {
            throw new Error(
                'CoreModule is already loaded. Import only in AppModule'
            );
        }

        // injector for decorators
        setStaticInjector(this.injector);
    }
}
