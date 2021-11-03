import { Injectable, FactoryProvider } from '@angular/core';
import { NGXLogger, NgxLoggerLevel, LoggerConfig } from 'ngx-logger';
import { StorageService, WellKnownLocalStorageKey } from './storage.service';

@Injectable({
    providedIn: 'root',
})
export class LoggerService {
    constructor(private logger: NGXLogger) {
        this.debug('[LoggerService] NGXLogger Created', logger);
        this.debug(
            `[LoggerService] Current logging level = ${
                NgxLoggerLevel[logger.getConfigSnapshot().level]
            }`
        );
    }

    trace(message: any, ...additional: any[]) {
        this.logger.trace(message, additional);
    }

    debug(message: any, ...additional: any[]) {
        this.logger.debug(message, additional);
    }

    info(message: any, ...additional: any[]) {
        this.logger.info(message, additional);
    }

    log(message: any, ...additional: any[]) {
        this.logger.log(message, additional);
    }

    warn(message: any, ...additional: any[]) {
        this.logger.warn(message, additional);
    }

    error(message: any, ...additional: any[]) {
        this.logger.error(message, additional);
    }

    fatal(message: any, ...additional: any[]) {
        this.logger.fatal(message, additional);
    }
}

export function LoggerConfigFactory(defaultLevel: NgxLoggerLevel, storageService: StorageService) {
    let level = storageService.get<number>(WellKnownLocalStorageKey.loggingLevel);
    if (level === null || level === undefined || Number.isNaN(level)) {
        level = defaultLevel;
    }

    return { level };
}

// eslint-disable-next-line @typescript-eslint/naming-convention
export function LOGGER_CONFIG_PROVIDER(development: boolean) {
    const defaultLevel = development ? NgxLoggerLevel.TRACE : NgxLoggerLevel.WARN;
    const provider: FactoryProvider = {
        provide: LoggerConfig,
        useFactory: LoggerConfigFactory.bind(null, defaultLevel),
        deps: [StorageService],
    };

    return provider;
}
