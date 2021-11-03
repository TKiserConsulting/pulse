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

import { InstructorEmoticonListItemDto } from '../models/instructor-emoticon-list-item-dto';
import { InstructorEmoticonsUpdateDto } from '../models/instructor-emoticons-update-dto';

@Injectable({
    providedIn: 'root',
})
export class EmoticonsService extends BaseService {
    public get name(): string { return 'EmoticonsService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation emoticonsList
   */
  static readonly EmoticonsListPath = '/api/1.0/emoticons';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `list()` instead.
   *
   * This method doesn't expect any request body.
   */
  list$Response(params?: {

    }): Observable<StrictHttpResponse<Array<InstructorEmoticonListItemDto>>> {

    const rb = new RequestBuilder(this.rootUrl, EmoticonsService.EmoticonsListPath, 'get');
    if (params) {


    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<Array<InstructorEmoticonListItemDto>>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `list$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  list(params?: {

    }): Observable<Array<InstructorEmoticonListItemDto>> {

      return this.list$Response(params).pipe(
          map((r: StrictHttpResponse<Array<InstructorEmoticonListItemDto>>) => r.body as Array<InstructorEmoticonListItemDto>)
      );
  }

  /**
   * Path part for operation emoticonsUpdate
   */
  static readonly EmoticonsUpdatePath = '/api/1.0/emoticons';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `update()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  update$Response(params: {

        body: InstructorEmoticonsUpdateDto
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, EmoticonsService.EmoticonsUpdatePath, 'put');
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

        body: InstructorEmoticonsUpdateDto
    }): Observable<void> {

      return this.update$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

}
