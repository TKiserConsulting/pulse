import { HttpClient } from '@angular/common/http';
import { FactoryProvider } from '@angular/core';
import { TranslateLoader } from '@ngx-translate/core';
import { AnonymousTranslateHttpLoader } from './anonymous-translate-http-loader';

export function HttpLoaderFactory(httpClient: HttpClient) {
    return new AnonymousTranslateHttpLoader(httpClient);
}

export const ANONYMOUS_TRANSLATE_LOADER_PROVIDER: FactoryProvider = {
    provide: TranslateLoader,
    useFactory: HttpLoaderFactory,
    deps: [HttpClient],
};
