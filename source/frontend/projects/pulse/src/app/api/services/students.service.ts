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

import { EmoticonTapDto } from '../models/emoticon-tap-dto';
import { JoinSessionRequestDto } from '../models/join-session-request-dto';
import { JoinSessionResultDto } from '../models/join-session-result-dto';
import { SessionQuestionCreateDto } from '../models/session-question-create-dto';
import { StudentSessionDetailsDto } from '../models/student-session-details-dto';

@Injectable({
    providedIn: 'root',
})
export class StudentsService extends BaseService {
    public get name(): string { return 'StudentsService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation studentsSignin
   */
  static readonly StudentsSigninPath = '/api/1.0/students/signin';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `signin()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  signin$Response(params: {

        body: JoinSessionRequestDto
    }): Observable<StrictHttpResponse<JoinSessionResultDto>> {

    const rb = new RequestBuilder(this.rootUrl, StudentsService.StudentsSigninPath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<JoinSessionResultDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `signin$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  signin(params: {

        body: JoinSessionRequestDto
    }): Observable<JoinSessionResultDto> {

      return this.signin$Response(params).pipe(
          map((r: StrictHttpResponse<JoinSessionResultDto>) => r.body as JoinSessionResultDto)
      );
  }

  /**
   * Path part for operation studentsGetSession
   */
  static readonly StudentsGetSessionPath = '/api/1.0/students/session';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `getSession()` instead.
   *
   * This method doesn't expect any request body.
   */
  getSession$Response(params?: {
    SessionId?: string;

    }): Observable<StrictHttpResponse<StudentSessionDetailsDto>> {

    const rb = new RequestBuilder(this.rootUrl, StudentsService.StudentsGetSessionPath, 'get');
    if (params) {

      rb.query('SessionId', params.SessionId);

    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<StudentSessionDetailsDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `getSession$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  getSession(params?: {
    SessionId?: string;

    }): Observable<StudentSessionDetailsDto> {

      return this.getSession$Response(params).pipe(
          map((r: StrictHttpResponse<StudentSessionDetailsDto>) => r.body as StudentSessionDetailsDto)
      );
  }

  /**
   * Path part for operation studentsTap
   */
  static readonly StudentsTapPath = '/api/1.0/students/tap';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `tap()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  tap$Response(params: {

        body: EmoticonTapDto
    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, StudentsService.StudentsTapPath, 'post');
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
   * To access the full response (for headers, for example), `tap$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  tap(params: {

        body: EmoticonTapDto
    }): Observable<void> {

      return this.tap$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

  /**
   * Path part for operation studentsCreateQuestion
   */
  static readonly StudentsCreateQuestionPath = '/api/1.0/students/question';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `createQuestion()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  createQuestion$Response(params: {

        body: SessionQuestionCreateDto
    }): Observable<StrictHttpResponse<string>> {

    const rb = new RequestBuilder(this.rootUrl, StudentsService.StudentsCreateQuestionPath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<string>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `createQuestion$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  createQuestion(params: {

        body: SessionQuestionCreateDto
    }): Observable<string> {

      return this.createQuestion$Response(params).pipe(
          map((r: StrictHttpResponse<string>) => r.body as string)
      );
  }

}
