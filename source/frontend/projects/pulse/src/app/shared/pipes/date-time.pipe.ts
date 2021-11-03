import { Pipe, PipeTransform } from '@angular/core';
import { LoggerService } from '@pulse/sdk';

@Pipe({ name: 'appDateTime' })
export class DateTimePipe implements PipeTransform {
    constructor(private loggerService: LoggerService) {}

    public static format(value: string, logger?: LoggerService) {
        let result = value;
        if (value) {
            try {
                const date = new Date(value);
                const options = {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric',

                    hour12: true,
                    hour: 'numeric',
                    minute: '2-digit',
                    second: '2-digit',
                } as any;

                result = new Intl.DateTimeFormat('en-US', options).format(date);
            } catch (e) {
                logger?.error('Unable to format date', e);
            }
        }

        return result;
    }

    public transform(value: string): string {
        const result = DateTimePipe.format(value, this.loggerService);
        return result;
    }
}
