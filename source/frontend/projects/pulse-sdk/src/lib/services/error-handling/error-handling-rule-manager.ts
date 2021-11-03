import { TranslateService } from '@ngx-translate/core';
import template from 'string-template';
import { ClientErrorModel, ErrorModel } from '../../models/ui-error.models';
import { ErrorHandlingValidationErrorConsumer } from './error-handling-validation-error-consumer';
import { ErrorHandlingRule } from './error-handling-rule';
import { ErrorHandlingErrorModel } from './error-handling-error-model';
import { ErrorHandlingManager } from './error-handling-manager';
import { LoggerService } from '../logger.service';

export class ErrorHandlingRuleManager implements ErrorHandlingManager {
    private defaultScope: string | null = null;

    private globalLocalizationPrefix = 'app.defaults.errors';

    private defaultLocalizationPrefix = '';

    private showToastWhenAllUnmapped = true;

    private showSummaryWhenAllUnmapped = true;

    private allUnmappedLocalizationKey = 'app.defaults.errors.UNEXPECTEDERROR';

    private rules: ErrorHandlingRule[] = [];

    public constructor(
        private loggerService: LoggerService,
        private translateService: TranslateService,
        private messageService: { add: (arg: any) => void } | /* MessageService */ null = null,
        private validationController: ErrorHandlingValidationErrorConsumer | null = null,
        private summaryControl: {
            setSummaryErrors: (m: ClientErrorModel[]) => void;
        } | null
    ) {}

    public scope(name: string): ErrorHandlingRuleManager {
        this.defaultScope = name;
        return this;
    }

    public l10nPrefix(prefix: string): ErrorHandlingRuleManager {
        this.defaultLocalizationPrefix = prefix || '';
        return this;
    }

    public toastWhenAllUnmapped(value: boolean = true): ErrorHandlingRuleManager {
        this.showToastWhenAllUnmapped = value;
        return this;
    }

    public map(rule: ErrorHandlingRule): ErrorHandlingRuleManager {
        if (rule.code === undefined && rule.propertyName === undefined) {
            this.map({ ...rule, code: '*' });
            this.map({ ...rule, code: '*', propertyName: '*' });
        } else {
            if (rule.propertyName && rule.code === undefined) {
                rule.code = '*';
            }

            if (rule.code && rule.propertyName && rule.validation === undefined) {
                rule.validation = true;
            }

            rule.orderIndex = rule.orderIndex || 1000;
            this.rules.push(rule);
        }

        return this;
    }

    public async populate(err: ErrorModel): Promise<void> {
        // this.rules = orderBy(this.rules, ['orderIndex'], ['asc']);

        this.loggerService.info('Error handling service catched an error', err);

        this.rules = (this.rules || []).sort((a, b) =>
            Math.sign((a.orderIndex || 0) - (b.orderIndex || 0))
        );
        const errors = this.expand(err);
        await this.enrich(errors);

        this.populateSummary(errors);
        this.populateValidation(errors);
        this.populateToast(errors);
    }

    private expand(err: ErrorModel): ErrorHandlingErrorModel[] {
        const list: ErrorHandlingErrorModel[] = [];

        if (err) {
            const errors = err.errors || {};
            if (Object.keys(errors).length === 0) {
                const parent = new ErrorHandlingErrorModel(
                    this.defaultScope || '',
                    err.code || 'UNEXPECTEDERROR',
                    err.title || err.message
                );
                list.push(parent);
            }

            Object.keys(errors).forEach((propertyName) => {
                Object.values(errors[propertyName]).forEach((entry) => {
                    const child = new ErrorHandlingErrorModel(
                        this.defaultScope || '',
                        entry.code,
                        entry.reason,
                        propertyName,
                        entry.extendedData
                    );
                    list.push(child);
                });
            });
        }

        return list;
    }

    private async enrich(errors: ErrorHandlingErrorModel[]): Promise<void> {
        Object.values(errors).forEach((e) => {
            e.rule = this.getAggregatedRule(e.code, e.propertyName || '');
            e.localizationKey = this.getLocalizationKey(e.rule);
        });

        const hasUiMapping = (errors || []).some(
            (e) =>
                e.rule &&
                ((e.rule.summary !== undefined && this.summaryControl) ||
                    (e.rule.validation !== undefined && this.validationController) ||
                    (e.rule.toast !== undefined && this.messageService))
        );

        if (!hasUiMapping && (this.showToastWhenAllUnmapped || this.showSummaryWhenAllUnmapped)) {
            const data = new ErrorHandlingErrorModel(this.defaultScope || '', 'UNEXPECTEDERROR');
            data.rule = {
                code: data.code,
                toast: this.showToastWhenAllUnmapped,
                summary: this.showSummaryWhenAllUnmapped,
                orderIndex: Number.MAX_VALUE,
            };
            data.localizationKey = this.allUnmappedLocalizationKey;
            errors.push(data);
        }

        const localizationKeys = (errors || []).map((e) => e.localizationKey || '');
        const localizationSet = await this.translate(localizationKeys);
        Object.values(errors).forEach((e) => {
            const message = localizationSet[e.localizationKey || ''];
            e.message = e.extendedData ? template(message, e.extendedData) : message;
        });
    }

    private getAggregatedRule(code: string, propertyName: string): ErrorHandlingRule {
        const rules = (this.rules || []).filter(
            (r: ErrorHandlingRule) =>
                (r.code === code || r.code === '*') &&
                ((propertyName && r.propertyName === propertyName) ||
                    (propertyName && r.propertyName === '*') ||
                    (propertyName === undefined && r.propertyName === undefined))
        );

        const rule: ErrorHandlingRule = {
            code,
            propertyName,
            orderIndex: Number.MAX_VALUE,
        };

        Object.values(rules).forEach((r) => {
            rule.orderIndex = Math.min(rule.orderIndex || 0, r.orderIndex || Number.MAX_VALUE);

            if (rule.localizationKey === undefined && r.localizationKey) {
                rule.localizationKey = r.localizationKey;
            }

            if (rule.toast === undefined && r.toast !== undefined) {
                rule.toast = r.toast;
            }

            if (rule.validation === undefined && r.validation !== undefined) {
                rule.validation = r.validation;
            }

            if (rule.summary === undefined && r.summary !== undefined) {
                rule.summary = r.summary;
            }
        });

        return rule;
    }

    private getLocalizationKey(rule: ErrorHandlingRule): string {
        let localizationKey = rule.localizationKey || this.buildDefaultLocalizationKey(rule);
        localizationKey = this.toAbsoluteLocalizationKey(localizationKey, rule);
        return localizationKey;
    }

    private buildDefaultLocalizationKey(rule: ErrorHandlingRule): string {
        const propertyName = rule.propertyName || '';
        const code = rule.code || '';
        let key = propertyName && code ? `${propertyName}_${code}` : `${propertyName}${code}`;
        key = key.replace(/[.]/g, '_');

        return key;
    }

    private toAbsoluteLocalizationKey(localizationKey: string, rule: ErrorHandlingRule): string {
        const localizationPrefix =
            rule.propertyName === undefined
                ? this.globalLocalizationPrefix
                : this.defaultLocalizationPrefix;

        localizationKey =
            localizationKey &&
            localizationKey.indexOf('.') === -1 &&
            localizationPrefix &&
            localizationPrefix !== ''
                ? `${localizationPrefix}.${localizationKey}`
                : localizationKey;
        return localizationKey;
    }

    private async translate(localizationKeys: string[]): Promise<{ [s: string]: string }> {
        const result: { [s: string]: string } = await this.translateService
            .get(localizationKeys)
            .toPromise();
        return result;
    }

    private populateSummary(errors: ErrorHandlingErrorModel[]) {
        if (this.summaryControl) {
            const items = (errors || []).filter((e) => e.rule && e.rule.summary === true);
            this.summaryControl.setSummaryErrors(items);
        }
    }

    private populateValidation(errors: ErrorHandlingErrorModel[]) {
        if (this.validationController) {
            const items = (errors || []).filter((e) => e.rule && e.rule.validation === true);
            this.validationController.setValidationErrors(items);
        }
    }

    private populateToast(errors: ErrorHandlingErrorModel[]) {
        if (this.messageService) {
            const items = (errors || []).filter((e) => e.rule && e.rule.toast === true);
            Object.values(items).forEach((item) => {
                this.messageService?.add({
                    severity: 'error',
                    detail: item.message,
                    // do we want to have it configurable?
                    sticky: true,
                });
            });
        }
    }
}
