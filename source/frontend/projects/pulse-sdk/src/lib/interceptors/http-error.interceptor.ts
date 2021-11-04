import { ClassProvider, Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpErrorResponse,
    HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, switchMap, map } from 'rxjs/operators';
import { LoggerService } from '../services/logger.service';
import { ApiErrorDescription, ApiError, ApiErrorType } from '../models';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
    constructor(private logger: LoggerService) {}

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            catchError((err: HttpErrorResponse) => {
                return this.throwApiError(err);
            })
        );
    }

    private throwApiError(errorResponse: HttpErrorResponse): Observable<never> {
        const obs: Observable<any> = of(errorResponse.error);
        return obs.pipe(
            map((error) => this.transformError(errorResponse, error)),
            switchMap((error) => throwError(error))
        );
    }

    private transformError(
        errorResponse: HttpErrorResponse,
        error: any
    ): ApiError {
        let errorDescription: ApiErrorDescription;
        try {
            if (!error) {
                throw new Error('The error depends from status only');
            }

            errorDescription =
                typeof error === 'string' ? JSON.parse(error) : error;
            if (!errorDescription.code) {
                this.logger.error('Unable to parse error', error);
                throw new Error('Invalid error object obtained');
            }
        } catch (e) {
            switch (errorResponse.status) {
                case 0:
                    errorDescription = {
                        message: 'Connection error',
                        code: ApiErrorType.ConnectionError,
                    };
                    break;
                case 401:
                    errorDescription = {
                        message: 'Signin required',
                        code: ApiErrorType.UnAuthenticated,
                    };
                    break;
                case 403:
                    errorDescription = {
                        message: 'Access denied',
                        code: ApiErrorType.AccessDenied,
                    };
                    break;
                case 404:
                    errorDescription = {
                        message: 'Requested resource not found',
                        code: ApiErrorType.ResourceNotFound,
                    };
                    break;
                case 504:
                    errorDescription = {
                        message: 'Server temporarry unavailable',
                        code: ApiErrorType.GatewayTimeout,
                    };
                    break;
                default:
                    errorDescription = {
                        message: 'Unexpected error occurred',
                        code: ApiErrorType.UnexpectedError,
                    };
                    break;
            }
        }

        const apiError = new ApiError();
        apiError.traceId = errorDescription.traceId || undefined;
        apiError.code = errorDescription.code || ApiErrorType.UnexpectedError;
        apiError.title =
            errorDescription.title ||
            errorDescription.message ||
            'Unexpected error occurred';
        apiError.message =
            errorDescription.message ||
            errorDescription.title ||
            'Unexpected error occurred';
        apiError.errors = errorDescription.errors || [];
        apiError.innerError = errorResponse;
        return apiError;
    }
}

export const HTTP_ERROR_INTERCEPTOR_PROVIDER: ClassProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: HttpErrorInterceptor,
    multi: true,
};
