import { Injectable } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';

import { Store } from '@ngrx/store';

import { Observable } from 'rxjs';
import { ConfigSettings } from '../models/config-settings.model';
import * as fromConfig from '../reducers/config.reducer';

@Injectable({ providedIn: 'root' })
export class ConfigService {
    private httpClient: HttpClient;

    public baseUrl = '/api/1.0';

    public constructor(private store: Store<any>, handler: HttpBackend) {
        this.httpClient = new HttpClient(handler);
    }

    public async initialize(): Promise<void> {
        const config = await this.getConfig().toPromise();
        if (config) {
            this.store.dispatch(fromConfig.changeConfig({ config }));
        }
    }

    private getConfig(): Observable<ConfigSettings> {
        return this.httpClient.get<ConfigSettings>(`${this.baseUrl}/config`);
    }
}
