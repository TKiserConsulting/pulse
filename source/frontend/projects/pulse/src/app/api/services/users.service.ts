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

import { FilteringData } from '../models/filtering-data';
import { UserCreateDto } from '../models/user-create-dto';
import { UserDetailsDto } from '../models/user-details-dto';
import { UserListItemDto } from '../models/user-list-item-dto';
import { UserUpdateDto } from '../models/user-update-dto';

@Injectable({
    providedIn: 'root',
})
export class UsersService extends BaseService {
    public get name(): string { return 'UsersService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation usersFind
   */
  static readonly UsersFindPath = '/api/1.0/users';

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

    }): Observable<StrictHttpResponse<Array<UserListItemDto>>> {

    const rb = new RequestBuilder(this.rootUrl, UsersService.UsersFindPath, 'get');
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
            return r as StrictHttpResponse<Array<UserListItemDto>>;
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

    }): Observable<Array<UserListItemDto>> {

      return this.find$Response(params).pipe(
          map((r: StrictHttpResponse<Array<UserListItemDto>>) => r.body as Array<UserListItemDto>)
      );
  }

  /**
   * Path part for operation usersCreate
   */
  static readonly UsersCreatePath = '/api/1.0/users';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `create()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  create$Response(params: {

        body: UserCreateDto
    }): Observable<StrictHttpResponse<UserDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, UsersService.UsersCreatePath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<UserDetailsDto>;
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

        body: UserCreateDto
    }): Observable<UserDetailsDto> {

      return this.create$Response(params).pipe(
          map((r: StrictHttpResponse<UserDetailsDto>) => r.body as UserDetailsDto)
      );
  }

  /**
   * Path part for operation usersRead
   */
  static readonly UsersReadPath = '/api/1.0/users/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `read()` instead.
   *
   * This method doesn't expect any request body.
   */
  read$Response(params: {
    id: string;

    }): Observable<StrictHttpResponse<UserDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, UsersService.UsersReadPath, 'get');
    if (params) {

      rb.path('id', params.id);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<UserDetailsDto>;
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

    }): Observable<UserDetailsDto> {

      return this.read$Response(params).pipe(
          map((r: StrictHttpResponse<UserDetailsDto>) => r.body as UserDetailsDto)
      );
  }

  /**
   * Path part for operation usersUpdate
   */
  static readonly UsersUpdatePath = '/api/1.0/users/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update$Response(params: {
    id: string;

        body: UserUpdateDto
    }): Observable<StrictHttpResponse<UserDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, UsersService.UsersUpdatePath, 'put');
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
            return r as StrictHttpResponse<UserDetailsDto>;
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

        body: UserUpdateDto
    }): Observable<UserDetailsDto> {

      return this.update$Response(params).pipe(
          map((r: StrictHttpResponse<UserDetailsDto>) => r.body as UserDetailsDto)
      );
  }

  /**
   * Path part for operation usersDelete
   */
  static readonly UsersDeletePath = '/api/1.0/users/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `delete()` instead.
   *
   * This method doesn't expect any request body.
   */
  delete$Response(params: {
    id: string;

    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, UsersService.UsersDeletePath, 'delete');
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
