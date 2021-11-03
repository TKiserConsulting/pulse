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

import { ActiveSessionDetailsDto } from '../models/active-session-details-dto';
import { FilteringData } from '../models/filtering-data';
import { SessionCreateDto } from '../models/session-create-dto';
import { SessionDetailsDto } from '../models/session-details-dto';
import { SessionListItemDto } from '../models/session-list-item-dto';

@Injectable({
    providedIn: 'root',
})
export class SessionsService extends BaseService {
    public get name(): string { return 'SessionsService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation sessionsGetActiveSession
   */
  static readonly SessionsGetActiveSessionPath = '/api/1.0/sessions/activesession';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getActiveSession()` instead.
   *
   * This method doesn't expect any request body.
   */
  getActiveSession$Response(params?: {

    }): Observable<StrictHttpResponse<ActiveSessionDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsGetActiveSessionPath, 'get');
    if (params) {


    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<ActiveSessionDetailsDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getActiveSession$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getActiveSession(params?: {

    }): Observable<ActiveSessionDetailsDto> {

      return this.getActiveSession$Response(params).pipe(
          map((r: StrictHttpResponse<ActiveSessionDetailsDto>) => r.body as ActiveSessionDetailsDto)
      );
  }

  /**
   * Path part for operation sessionsEndSession
   */
  static readonly SessionsEndSessionPath = '/api/1.0/sessions/endsession';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `endSession()` instead.
   *
   * This method doesn't expect any request body.
   */
  endSession$Response(params?: {

    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsEndSessionPath, 'post');
    if (params) {


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
   * To access the full response (for headers, for example), `endSession$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  endSession(params?: {

    }): Observable<void> {

      return this.endSession$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

  /**
   * Path part for operation sessionsFind
   */
  static readonly SessionsFindPath = '/api/1.0/sessions';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `find()` instead.
   *
   * This method doesn't expect any request body.
   */
  find$Response(params?: {
    range?: string;
    'range-disable-total'?: boolean;
    filterBy?: FilteringData;

    }): Observable<StrictHttpResponse<Array<SessionListItemDto>>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsFindPath, 'get');
    if (params) {

      rb.header('range', params.range);
      rb.header('range-disable-total', params['range-disable-total']);
      rb.query('filterBy', params.filterBy);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<Array<SessionListItemDto>>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `find$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  find(params?: {
    range?: string;
    'range-disable-total'?: boolean;
    filterBy?: FilteringData;

    }): Observable<Array<SessionListItemDto>> {

      return this.find$Response(params).pipe(
          map((r: StrictHttpResponse<Array<SessionListItemDto>>) => r.body as Array<SessionListItemDto>)
      );
  }

  /**
   * Path part for operation sessionsCreate
   */
  static readonly SessionsCreatePath = '/api/1.0/sessions';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `create()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  create$Response(params: {

        body: SessionCreateDto
    }): Observable<StrictHttpResponse<SessionDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsCreatePath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<SessionDetailsDto>;
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

        body: SessionCreateDto
    }): Observable<SessionDetailsDto> {

      return this.create$Response(params).pipe(
          map((r: StrictHttpResponse<SessionDetailsDto>) => r.body as SessionDetailsDto)
      );
  }

  /**
   * Path part for operation sessionsRead
   */
  static readonly SessionsReadPath = '/api/1.0/sessions/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `read()` instead.
   *
   * This method doesn't expect any request body.
   */
  read$Response(params: {
    id: string;

    }): Observable<StrictHttpResponse<SessionDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsReadPath, 'get');
    if (params) {

      rb.path('id', params.id);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<SessionDetailsDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `read$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  read(params: {
    id: string;

    }): Observable<SessionDetailsDto> {

      return this.read$Response(params).pipe(
          map((r: StrictHttpResponse<SessionDetailsDto>) => r.body as SessionDetailsDto)
      );
  }

  /**
   * Path part for operation sessionsUpdate
   */
  static readonly SessionsUpdatePath = '/api/1.0/sessions/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update$Response(params: {
    id: string;

        body: SessionCreateDto
    }): Observable<StrictHttpResponse<SessionDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsUpdatePath, 'put');
    if (params) {

      rb.path('id', params.id);

      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<SessionDetailsDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `update$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update(params: {
    id: string;

        body: SessionCreateDto
    }): Observable<SessionDetailsDto> {

      return this.update$Response(params).pipe(
          map((r: StrictHttpResponse<SessionDetailsDto>) => r.body as SessionDetailsDto)
      );
  }

  /**
   * Path part for operation sessionsDelete
   */
  static readonly SessionsDeletePath = '/api/1.0/sessions/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `delete()` instead.
   *
   * This method doesn't expect any request body.
   */
  delete$Response(params: {
    id: string;

    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, SessionsService.SessionsDeletePath, 'delete');
    if (params) {

      rb.path('id', params.id);

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
   * To access the full response (for headers, for example), `delete$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  delete(params: {
    id: string;

    }): Observable<void> {

      return this.delete$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

}
