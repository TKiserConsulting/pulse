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

import { GrantDescriptor } from '../models/grant-descriptor';
import { GrantResult } from '../models/grant-result';

@Injectable({
    providedIn: 'root',
})
export class AclService extends BaseService {
    public get name(): string { return 'AclService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation aclCheckSingle
   */
  static readonly AclCheckSinglePath = '/api/1.0/acl/check';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `checkSingle()` instead.
   *
   * This method doesn't expect any request body.
   */
  checkSingle$Response(params: {
    grantType: string;
    grantAction: string;

    }): Observable<StrictHttpResponse<GrantResult>> {

    const rb = new RequestBuilder(this.rootUrl, AclService.AclCheckSinglePath, 'get');
    if (params) {

      rb.query('grantType', params.grantType);
      rb.query('grantAction', params.grantAction);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<GrantResult>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `checkSingle$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  checkSingle(params: {
    grantType: string;
    grantAction: string;

    }): Observable<GrantResult> {

      return this.checkSingle$Response(params).pipe(
          map((r: StrictHttpResponse<GrantResult>) => r.body as GrantResult)
      );
  }

  /**
   * Path part for operation aclCheckMany
   */
  static readonly AclCheckManyPath = '/api/1.0/acl/check-many';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `checkMany()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  checkMany$Response(params: {

        body: Array<GrantDescriptor>
    }): Observable<StrictHttpResponse<GrantResult>> {

    const rb = new RequestBuilder(this.rootUrl, AclService.AclCheckManyPath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<GrantResult>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `checkMany$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  checkMany(params: {

        body: Array<GrantDescriptor>
    }): Observable<GrantResult> {

      return this.checkMany$Response(params).pipe(
          map((r: StrictHttpResponse<GrantResult>) => r.body as GrantResult)
      );
  }

}
