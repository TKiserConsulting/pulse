import { ClassProvider, Injectable } from '@angular/core';
import {
    HttpInterceptor,
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HTTP_INTERCEPTORS,
} from '@angular/common/http';

import { Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class HttpLocalizationInterceptor implements HttpInterceptor {
    constructor(private translateService: TranslateService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!request.headers.has('Accept-Language')) {
            const lang = this.translateService.currentLang || this.translateService.defaultLang;
            if (lang) {
                request = request.clone({ headers: request.headers.set('Accept-Language', lang) });
            }
        }

        return next.handle(request);
    }
}

export const HTTP_LOCALIZATION_INTERCEPTOR_PROVIDER: ClassProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: HttpLocalizationInterceptor,
    multi: true,
};
