import { Injectable, Inject, InjectionToken } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup } from '@angular/forms';
import { from, Observable, of, OperatorFunction, ObservedValueOf } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { ClientErrorModel, ErrorModel } from '../models/ui-error.models';
import { ErrorHandlingValidationErrorConsumer } from './error-handling/error-handling-validation-error-consumer';
import { ErrorHandlingRule } from './error-handling/error-handling-rule';
import { ErrorHandlingRuleManager } from './error-handling/error-handling-rule-manager';
import { populateClientErrors } from '../helpers/form.helpers';
import { LoggerService } from './logger.service';

export const ERROR_HANDLING_MESSAGE_SERVICE =
    new InjectionToken<ErrorHandlingService>('MessageService');

@Injectable({ providedIn: 'root' })
export class ErrorHandlingService {
    public constructor(
        private translateService: TranslateService,
        private loggerService: LoggerService,
        @Inject(ERROR_HANDLING_MESSAGE_SERVICE)
        private defaultMessageService: any // MessageService
    ) {}

    public call$<T>(
        producer: Promise<T> | Observable<T>,
        alternateInstanceFn: () => T,
        validationPrefix?: string,
        validation?: ErrorHandlingValidationErrorConsumer | FormGroup
    ): Observable<T> {
        const obs = <Observable<T>>from(producer);
        return obs.pipe(
            this.catchErrorUseDefault$(
                alternateInstanceFn,
                validationPrefix,
                validation
            )
        );
    }

    public catchErrorUseDefault$<T>(
        alternateInstanceFn: () => T,
        validationPrefix?: string,
        validation?: ErrorHandlingValidationErrorConsumer | FormGroup
    ): OperatorFunction<T, T | ObservedValueOf<void>> {
        return catchError((err) =>
            from(this.populateDefault(err, validationPrefix, validation)).pipe(
                switchMap(() => of(alternateInstanceFn()))
            )
        );
    }

    public populateDefault(
        err: ErrorModel,
        validationPrefix?: string,
        validation?: ErrorHandlingValidationErrorConsumer | FormGroup
    ): Promise<void> {
        const propertyNameRule: ErrorHandlingRule = {
            propertyName: '*',
        };

        const codeNameRule: ErrorHandlingRule = {
            code: '*',
            toast: true,
        };

        if (!validation) {
            propertyNameRule.toast = true;
        }

        return this.default(validation)
            .l10nPrefix(validationPrefix || '')
            .map(propertyNameRule)
            .map(codeNameRule)
            .populate(err);
    }

    public default(
        validation:
            | ErrorHandlingValidationErrorConsumer
            | FormGroup
            | undefined,
        summary: {
            setSummaryErrors: (m: ClientErrorModel[]) => void;
        } | null = null
    ): ErrorHandlingRuleManager {
        const validationInstance = this.mapValidator(validation);
        return new ErrorHandlingRuleManager(
            this.loggerService,
            this.translateService,
            this.defaultMessageService,
            validationInstance,
            summary
        );
    }

    private mapValidator(
        consumer: ErrorHandlingValidationErrorConsumer | FormGroup | undefined
    ): ErrorHandlingValidationErrorConsumer | null {
        let instance: ErrorHandlingValidationErrorConsumer | null = null;
        if (consumer != null) {
            if (consumer instanceof FormGroup) {
                instance = {
                    setValidationErrors: populateClientErrors.bind(
                        null,
                        consumer
                    ),
                };
            } else {
                instance = consumer as ErrorHandlingValidationErrorConsumer;
            }
        }
        return instance;
    }
}
