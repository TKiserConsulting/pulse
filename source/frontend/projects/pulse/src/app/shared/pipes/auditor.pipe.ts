import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'appAuditor' })
export class AuditorPipe implements PipeTransform {
    public transform(value: string): string {
        let result = '';
        if (value) {
            const parts = value.split(':');
            if (parts.length > 1) {
                result = parts[1] || parts[0];
            } else if (parts.length > 0) {
                [result] = parts;
            }
        }

        return result;
    }
}
