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

import { SessionQuestionDetailsDto } from '../models/session-question-details-dto';
import { SessionQuestionDismissDto } from '../models/session-question-dismiss-dto';

@Injectable({
    providedIn: 'root',
})
export class QuestionsService extends BaseService {
    public get name(): string { return 'QuestionsService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation questionsList
   */
  static readonly QuestionsListPath = '/api/1.0/questions';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `list()` instead.
   *
   * This method doesn't expect any request body.
   */
  list$Response(params?: {
    sessionCheckinId?: string;

    }): Observable<StrictHttpResponse<Array<SessionQuestionDetailsDto>>> {

    const rb = new RequestBuilder(this.rootUrl, QuestionsService.QuestionsListPath, 'get');
    if (params) {

      rb.query('sessionCheckinId', params.sessionCheckinId);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<Array<SessionQuestionDetailsDto>>;
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
    sessionCheckinId?: string;

    }): Observable<Array<SessionQuestionDetailsDto>> {

      return this.list$Response(params).pipe(
          map((r: StrictHttpResponse<Array<SessionQuestionDetailsDto>>) => r.body as Array<SessionQuestionDetailsDto>)
      );
  }

  /**
   * Path part for operation questionsDismiss
   */
  static readonly QuestionsDismissPath = '/api/1.0/questions/dismiss';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `dismiss()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  dismiss$Response(params: {

        body: SessionQuestionDismissDto
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, QuestionsService.QuestionsDismissPath, 'put');
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
   * To access the full response (for headers, for example), `dismiss$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  dismiss(params: {

        body: SessionQuestionDismissDto
    }): Observable<void> {

      return this.dismiss$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

}
