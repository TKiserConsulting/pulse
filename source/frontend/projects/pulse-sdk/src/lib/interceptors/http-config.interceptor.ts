import { ClassProvider, Injectable } from '@angular/core';
import {
    HttpInterceptor,
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HTTP_INTERCEPTORS,
} from '@angular/common/http';

import { Observable } from 'rxjs';
import { HttpConfigService } from '../services/http-config.service';

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {
    constructor(private configService: HttpConfigService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (!request.headers.has('Content-Type')) {
            request = request.clone({
                headers: request.headers.set('Content-Type', 'application/json'),
            });
        }

        if (!request.headers.has('Accept')) {
            request = request.clone({ headers: request.headers.set('Accept', 'application/json') });
        }

        request = this.configService.apply(request);

        return next.handle(request);
    }
}

export const HTTP_CONFIG_INTERCEPTOR_PROVIDER: ClassProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: HttpConfigInterceptor,
    multi: true,
};
