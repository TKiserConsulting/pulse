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

import { SessionCheckinCreateDto } from '../models/session-checkin-create-dto';
import { SessionCheckinDetailsDto } from '../models/session-checkin-details-dto';
import { SessionCheckinFinishDto } from '../models/session-checkin-finish-dto';

@Injectable({
    providedIn: 'root',
})
export class CheckinService extends BaseService {
    public get name(): string { return 'CheckinService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation checkinCreate
   */
  static readonly CheckinCreatePath = '/api/1.0/checkin/checkin';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `create()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  create$Response(params: {

        body: SessionCheckinCreateDto
    }): Observable<StrictHttpResponse<SessionCheckinDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, CheckinService.CheckinCreatePath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<SessionCheckinDetailsDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `create$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  create(params: {

        body: SessionCheckinCreateDto
    }): Observable<SessionCheckinDetailsDto> {

      return this.create$Response(params).pipe(
          map((r: StrictHttpResponse<SessionCheckinDetailsDto>) => r.body as SessionCheckinDetailsDto)
      );
  }

  /**
   * Path part for operation checkinFinish
   */
  static readonly CheckinFinishPath = '/api/1.0/checkin/finish';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `finish()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  finish$Response(params: {

        body: SessionCheckinFinishDto
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, CheckinService.CheckinFinishPath, 'put');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'text',
        accept: '*/*'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `finish$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  finish(params: {

        body: SessionCheckinFinishDto
    }): Observable<void> {

      return this.finish$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

}
