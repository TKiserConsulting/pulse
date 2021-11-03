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

import { IHeaderDictionary } from '../models/i-header-dictionary';
import { InstructorProfileDetailsDto } from '../models/instructor-profile-details-dto';
import { InstructorProfileUpdateDto } from '../models/instructor-profile-update-dto';

@Injectable({
    providedIn: 'root',
})
export class ProfilesService extends BaseService {
    public get name(): string { return 'ProfilesService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation profilesLoad
   */
  static readonly ProfilesLoadPath = '/api/1.0/profiles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `load()` instead.
   *
   * This method doesn't expect any request body.
   */
  load$Response(params?: {

    }): Observable<StrictHttpResponse<InstructorProfileDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, ProfilesService.ProfilesLoadPath, 'get');
    if (params) {


    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<InstructorProfileDetailsDto>;
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

    }): Observable<InstructorProfileDetailsDto> {

      return this.load$Response(params).pipe(
          map((r: StrictHttpResponse<InstructorProfileDetailsDto>) => r.body as InstructorProfileDetailsDto)
      );
  }

  /**
   * Path part for operation profilesUpdate
   */
  static readonly ProfilesUpdatePath = '/api/1.0/profiles';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update$Response(params: {

        body: InstructorProfileUpdateDto
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProfilesService.ProfilesUpdatePath, 'post');
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

        body: InstructorProfileUpdateDto
    }): Observable<void> {

      return this.update$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

  /**
   * Path part for operation profilesLoadImage
   */
  static readonly ProfilesLoadImagePath = '/api/1.0/profiles/image';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `loadImage()` instead.
   *
   * This method doesn't expect any request body.
   */
  loadImage$Response(params?: {

    }): Observable<StrictHttpResponse<Blob>> {

    const rb = new RequestBuilder(this.rootUrl, ProfilesService.ProfilesLoadImagePath, 'get');
    if (params) {


    }
    return this.http.request(rb.build({
        responseType: 'blob',
        accept: 'application/octet-stream'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<Blob>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `loadImage$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  loadImage(params?: {

    }): Observable<Blob> {

      return this.loadImage$Response(params).pipe(
          map((r: StrictHttpResponse<Blob>) => r.body as Blob)
      );
  }

  /**
   * Path part for operation profilesUploadImage
   */
  static readonly ProfilesUploadImagePath = '/api/1.0/profiles/image';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `uploadImage()` instead.
   *
   * This method sends `multipart/form-data` and handles request body of type `multipart/form-data`.
   */
  uploadImage$Response(params?: {

        body?: {
'ContentType'?: string | null;
'ContentDisposition'?: string | null;
'Headers'?: IHeaderDictionary | null;
'Length'?: number;
'Name'?: string | null;
'FileName'?: string | null;
}
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, ProfilesService.ProfilesUploadImagePath, 'post');
    if (params) {


      rb.body(params.body, 'multipart/form-data');
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
   * To access the full response (for headers, for example), `uploadImage$Response()` instead.
   *
   * This method sends `multipart/form-data` and handles request body of type `multipart/form-data`.
   */
  uploadImage(params?: {

        body?: {
'ContentType'?: string | null;
'ContentDisposition'?: string | null;
'Headers'?: IHeaderDictionary | null;
'Length'?: number;
'Name'?: string | null;
'FileName'?: string | null;
}
    }): Observable<void> {

      return this.uploadImage$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

  /**
   * Path part for operation profilesLoadSmallImage
   */
  static readonly ProfilesLoadSmallImagePath = '/api/1.0/profiles/smallimage';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `loadSmallImage()` instead.
   *
   * This method doesn't expect any request body.
   */
  loadSmallImage$Response(params?: {

    }): Observable<StrictHttpResponse<Blob>> {

    const rb = new RequestBuilder(this.rootUrl, ProfilesService.ProfilesLoadSmallImagePath, 'get');
    if (params) {


    }
    return this.http.request(rb.build({
        responseType: 'blob',
        accept: 'application/octet-stream'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<Blob>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `loadSmallImage$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  loadSmallImage(params?: {

    }): Observable<Blob> {

      return this.loadSmallImage$Response(params).pipe(
          map((r: StrictHttpResponse<Blob>) => r.body as Blob)
      );
  }

}
