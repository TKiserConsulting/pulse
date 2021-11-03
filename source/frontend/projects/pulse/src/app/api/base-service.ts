/* tslint:disable */
/* eslint-disable */
import { ApiConfiguration } from './api-configuration';
import { HttpClient } from '@angular/common/http';
import { ApiService } from '@app/shared/services/api.service';

/**
 * Base class for services
 */
export abstract class BaseService extends ApiService  {
    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

    private _rootUrl: string = '';

    /**
     * Returns the root url for all operations in this service. If not set directly in this
     * service, will fallback to `ApiConfiguration.rootUrl`.
     */
    get rootUrl(): string {
        return this._rootUrl || this.config.rootUrl;
    }

    /**
     * Sets the root URL for API operations in this service.
     */
    set rootUrl(rootUrl: string) {
        this._rootUrl = rootUrl;
    }
}
