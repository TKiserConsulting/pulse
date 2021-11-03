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

import { ClassDetailsDto } from '../models/class-details-dto';
import { ClassListItemDto } from '../models/class-list-item-dto';
import { ClassUpsertDto } from '../models/class-upsert-dto';
import { FilteringData } from '../models/filtering-data';

@Injectable({
    providedIn: 'root',
})
export class ClassesService extends BaseService {
    public get name(): string { return 'ClassesService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation classesFind
   */
  static readonly ClassesFindPath = '/api/1.0/classes';

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

    }): Observable<StrictHttpResponse<Array<ClassListItemDto>>> {

    const rb = new RequestBuilder(this.rootUrl, ClassesService.ClassesFindPath, 'get');
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
            return r as StrictHttpResponse<Array<ClassListItemDto>>;
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

    }): Observable<Array<ClassListItemDto>> {

      return this.find$Response(params).pipe(
          map((r: StrictHttpResponse<Array<ClassListItemDto>>) => r.body as Array<ClassListItemDto>)
      );
  }

  /**
   * Path part for operation classesCreate
   */
  static readonly ClassesCreatePath = '/api/1.0/classes';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `create()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  create$Response(params: {

        body: ClassUpsertDto
    }): Observable<StrictHttpResponse<ClassDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, ClassesService.ClassesCreatePath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<ClassDetailsDto>;
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

        body: ClassUpsertDto
    }): Observable<ClassDetailsDto> {

      return this.create$Response(params).pipe(
          map((r: StrictHttpResponse<ClassDetailsDto>) => r.body as ClassDetailsDto)
      );
  }

  /**
   * Path part for operation classesRead
   */
  static readonly ClassesReadPath = '/api/1.0/classes/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `read()` instead.
   *
   * This method doesn't expect any request body.
   */
  read$Response(params: {
    id: string;

    }): Observable<StrictHttpResponse<ClassDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, ClassesService.ClassesReadPath, 'get');
    if (params) {

      rb.path('id', params.id);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<ClassDetailsDto>;
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

    }): Observable<ClassDetailsDto> {

      return this.read$Response(params).pipe(
          map((r: StrictHttpResponse<ClassDetailsDto>) => r.body as ClassDetailsDto)
      );
  }

  /**
   * Path part for operation classesUpdate
   */
  static readonly ClassesUpdatePath = '/api/1.0/classes/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update$Response(params: {
    id: string;

        body: ClassUpsertDto
    }): Observable<StrictHttpResponse<ClassDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, ClassesService.ClassesUpdatePath, 'put');
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
            return r as StrictHttpResponse<ClassDetailsDto>;
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

        body: ClassUpsertDto
    }): Observable<ClassDetailsDto> {

      return this.update$Response(params).pipe(
          map((r: StrictHttpResponse<ClassDetailsDto>) => r.body as ClassDetailsDto)
      );
  }

  /**
   * Path part for operation classesDelete
   */
  static readonly ClassesDeletePath = '/api/1.0/classes/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `delete()` instead.
   *
   * This method doesn't expect any request body.
   */
  delete$Response(params: {
    id: string;

    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ClassesService.ClassesDeletePath, 'delete');
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
