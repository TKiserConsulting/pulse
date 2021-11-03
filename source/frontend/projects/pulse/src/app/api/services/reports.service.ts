/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { SessionReportResultDto } from '../models/session-report-result-dto';

@Injectable({
    providedIn: 'root',
})
export class ReportsService extends BaseService {
    public get name(): string { return 'ReportsService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation reportsSessionReport
   */
  static readonly ReportsSessionReportPath = '/api/1.0/reports/session';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `sessionReport()` instead.
   *
   * This method doesn't expect any request body.
   */
  sessionReport$Response(params?: {
    sessionId?: string;

    }): Observable<StrictHttpResponse<SessionReportResultDto>> {

    const rb = new RequestBuilder(this.rootUrl, ReportsService.ReportsSessionReportPath, 'get');
    if (params) {

      rb.query('sessionId', params.sessionId);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<SessionReportResultDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `sessionReport$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  sessionReport(params?: {
    sessionId?: string;

    }): Observable<SessionReportResultDto> {

      return this.sessionReport$Response(params).pipe(
          map((r: StrictHttpResponse<SessionReportResultDto>) => r.body as SessionReportResultDto)
      );
  }

}
