import { FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ClientErrorModel } from '../models/ui-error.models';

const groupBy = <T>(items: T[], key: string): { [k: string]: T[] } =>
    (items || []).reduce(
        (result: any, item: any) => ({
            ...result,
            [item[key]]: [...(result[item[key]] || []), item],
        }),
        {}
    );

export function markAllFormFieldsAsDirty(formGroup: FormGroup): void {
    if (!formGroup) {
        throw new Error('formGroup cannot be null');
    }

    Object.keys(formGroup.controls || []).forEach((key) => {
        formGroup.controls[key].markAsDirty();
    });
}

export function populateClientErrors(
    frm: FormGroup,
    clientErrors: ClientErrorModel[]
): void {
    const errorSet = groupBy(clientErrors || [], 'propertyName');

    Object.keys(errorSet || {}).forEach((propertyName) => {
        const errors = (errorSet[propertyName] || []).reduce((r: any, e) => {
            r[e.code] = { message: e.message };
            return r;
        }, {});

        const cntrl = frm.controls[propertyName];
        if (cntrl) {
            cntrl.setErrors(errors);
        } else {
            // eslint-disable-next-line no-console
            console.warn(
                `Cannot bind validation error to control: ${propertyName}`
            );
        }
    });
}

export function matchValidator(matchFrom: string, matchTo: string) {
    return (group: FormGroup): any => {
        const control = group.controls[matchFrom];
        const matchingControl = group.controls[matchTo];

        if (matchingControl.errors && !matchingControl.errors.mustMatch) {
            return;
        }

        if (control.value !== matchingControl.value) {
            matchingControl.setErrors({ mustMatch: true });
        } else {
            matchingControl.setErrors(null);
        }
    };
}

export function getGridColumnsTranslation(
    translateService: TranslateService,
    key: string
): Observable<any> {
    return translateService.get(key).pipe(
        map((columns) => {
            const list = Object.keys(columns).map((c) => {
                const part = (columns[c] as string).split('|');
                const header = part[0];
                const title = part.length > 1 ? part[1] : header;
                return { field: c, header, title };
            });

            return list;
        })
    );
}
