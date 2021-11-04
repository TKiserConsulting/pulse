import { Injectable, Inject } from '@angular/core';

import * as webstorage from 'ngx-webstorage-service';

export enum WellKnownLocalStorageKey {
    language = 'language',
    loggingLevel = 'logging-level',
}

@Injectable({ providedIn: 'root' })
export class StorageService {
    private keyPrefix: string;

    constructor(
        @Inject(webstorage.SESSION_STORAGE)
        private storage: webstorage.StorageService
    ) {
        // eslint-disable-next-line no-undef
        this.keyPrefix = `pulse.storage:${window.location.origin}`;
    }

    public get<T>(key: WellKnownLocalStorageKey | any): T {
        return this.storage.get(`${this.keyPrefix}:${key}`) as T;
    }

    public set<T>(key: WellKnownLocalStorageKey | any, data: T) {
        this.storage.set(`${this.keyPrefix}:${key}`, data);
    }
}
