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

import { InstructorSettingsDetailsDto } from '../models/instructor-settings-details-dto';
import { InstructorSettingsUpdateDto } from '../models/instructor-settings-update-dto';

@Injectable({
    providedIn: 'root',
})
export class InstructorSettingsService extends BaseService {
    public get name(): string { return 'InstructorSettingsService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation instructorSettingsLoad
   */
  static readonly InstructorSettingsLoadPath = '/api/1.0/instructorsettings';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `load()` instead.
   *
   * This method doesn't expect any request body.
   */
  load$Response(params?: {

    }): Observable<StrictHttpResponse<InstructorSettingsDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, InstructorSettingsService.InstructorSettingsLoadPath, 'get');
    if (params) {


    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<InstructorSettingsDetailsDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `load$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  load(params?: {

    }): Observable<InstructorSettingsDetailsDto> {

      return this.load$Response(params).pipe(
          map((r: StrictHttpResponse<InstructorSettingsDetailsDto>) => r.body as InstructorSettingsDetailsDto)
      );
  }

  /**
   * Path part for operation instructorSettingsUpdate
   */
  static readonly InstructorSettingsUpdatePath = '/api/1.0/instructorsettings';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update$Response(params: {

        body: InstructorSettingsUpdateDto
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, InstructorSettingsService.InstructorSettingsUpdatePath, 'post');
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
   * To access the full response (for headers, for example), `update$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update(params: {

        body: InstructorSettingsUpdateDto
    }): Observable<void> {

      return this.update$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

}
