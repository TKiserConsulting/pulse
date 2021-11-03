/* eslint-disable max-classes-per-file */

import { BehaviorSubject } from 'rxjs';

export interface RangeDescription {
    unit: string;
    first: number | null;
    last: number | null;
    length: number | null;
}

export interface RangedResult<T> {
    range: RangeDescription | null;
    data: T[] | undefined;
}

export class RangedRequest {
    skip? = 0;

    take? = 10;

    disableTotal? = false;

    // todo [md]: should not 'sortDir' be an array as 'sortBy' is
    sortDir?: string;

    sortBy?: string[];

    filterBy: object | null = null;
}

export class GridBinder {
    public loading$ = new BehaviorSubject<boolean>(false);

    public total: number = 0;

    public filter: {
        first: number;
        rows: number;
        sortField: string | null;
        sortOrder: number;
    } = {
        first: 0,
        rows: 10,
        sortField: null,
        sortOrder: 1, // asc
    };

    public filterBy: object | null = null;

    public cols?: any[] = [];

    public toRangedRequest(): RangedRequest {
        const req: RangedRequest = new RangedRequest();
        req.take = this.filter.rows;
        req.skip = this.filter.first;
        req.disableTotal = !!this.total;
        req.sortBy = (this.filter.sortField || '').split(',');
        req.filterBy = this.filterBy;
        req.sortDir = this.filter.sortOrder === 1 ? 'asc' : 'desc';

        return req;
    }
}

export const DefaultRangeTakeLimit = 100;
