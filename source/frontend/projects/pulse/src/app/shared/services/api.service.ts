import { HttpClient } from '@angular/common/http';
import { ApiConfiguration } from '@app/api/api-configuration';
import { FilteringData } from '@app/api/models/filtering-data';
import { StrictHttpResponse } from '@app/api/strict-http-response';
import {
    RangeDescription,
    RangedRequest,
    RangedResult,
} from '@app/shared/models/common.models';
import { HttpConfigService } from '@pulse/sdk';
import { LazyLoadEvent } from 'primeng/api';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { StaticInjector } from './inject.service';

export abstract class ApiService {
    public abstract get name(): string;

    constructor(
        protected config: ApiConfiguration,
        protected http: HttpClient
    ) {}

    private static toRangeQuery(req: RangedRequest, name?: string): string {
        name = name || 'x-items';
        const rangeQuery = `${name}=${req.skip}-${
            req.skip || 0 + (req.take || 0) - 1
        }`;
        return rangeQuery;
    }

    private static parseRangeHeader(str: string): RangeDescription | null {
        let matches = str.match(/^([\w-]+) (\d+)-(\d+)\/(\d+|\*)/);
        let result: RangeDescription | null = !matches
            ? null
            : {
                  unit: matches[1],
                  first: +matches[2],
                  last: +matches[3],
                  length: matches[4] === '*' ? null : +matches[4],
              };

        if (!result) {
            matches = str.match(/^([\w-]+) \*\/(\d+|\*)/);
            result = !matches
                ? null
                : {
                      unit: matches[1],
                      first: null,
                      last: null,
                      length: matches[2] === '*' ? null : +matches[2],
                  };
        }

        return result;
    }

    public toRangedRequestFromLazyLoadEvent(
        event: LazyLoadEvent,
        disableTotal?: boolean
    ) {
        let filters: any = null;
        if (event.filters) {
            filters = {};
            Object.keys(event.filters).forEach((key) => {
                filters[key] = event.filters ? event.filters[key].value : null;
            });
        }

        return this.toRangedRequest(event, filters, disableTotal);
    }

    public toRangedRequest(
        event: {
            first?: number;
            rows?: number;
            sortField?: string;
            sortOrder?: number;
        },
        filterBy: FilteringData | null,
        disableTotal?: boolean
    ): RangedRequest {
        const req: RangedRequest = new RangedRequest();
        req.skip = event.first;
        req.take = event.rows;
        req.disableTotal = !!disableTotal;

        req.sortBy = (event.sortField || '').split(',');
        req.sortDir = event.sortOrder === 1 ? 'asc' : 'desc';
        req.filterBy = filterBy;

        return req;
    }

    public async queryRanged<T>(
        request: RangedRequest,
        method: string | null
    ): Promise<RangedResult<T>> {
        method = `${method || 'find'}$Response`;
        const api = this;
        const action: (params?: {
            range: string;
            'range-disable-total'?: boolean;
            filterBy: object | null;
            sortBy?: string[];
            sortDir?: string;
        }) => Observable<StrictHttpResponse<Array<T>>> = (api as any)[
            method
        ].bind(api);

        const arg = {
            ...request,
            range: ApiService.toRangeQuery(request, this.name),
            'range-disable-total': request.disableTotal,
        };

        const observable = action(arg);
        const res = await observable.toPromise();
        const items = res?.body;
        const value = res?.headers.get('content-range');
        const range = ApiService.parseRangeHeader(value || '');
        return { range, data: items };
    }

    public multipart(params?: any, method?: string): Observable<any> {
        method = `${method || 'upload'}$Response`;
        const api = this;

        const httpConfigService = StaticInjector.get(HttpConfigService);

        const action: (params?: any) => Observable<StrictHttpResponse<any>> = (
            api as any
        )[method].bind(api);

        // delete content type to support multipart content type header
        httpConfigService.register((req) => {
            req = req.clone({
                headers: req.headers.delete('Content-Type'),
            });
            return req;
        });

        const observable = action(params).pipe(
            finalize(() => {
                // clear handler
                httpConfigService.clear();
            })
        );

        return observable;
    }
}
