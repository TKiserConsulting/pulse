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

import { RefreshRequestDto } from '../models/refresh-request-dto';
import { RefreshResultDto } from '../models/refresh-result-dto';
import { RegisterRequestDto } from '../models/register-request-dto';
import { RegisterResultDto } from '../models/register-result-dto';
import { SigninRequestDto } from '../models/signin-request-dto';
import { SigninResultDto } from '../models/signin-result-dto';

@Injectable({
    providedIn: 'root',
})
export class AuthenticationService extends BaseService {
    public get name(): string { return 'AuthenticationService'.replace('Service', '').replace(/^\w/, c => c.toLowerCase()); };

    constructor(
        config: ApiConfiguration,
        http: HttpClient
    ) {
        super(config, http);
    }

  /**
   * Path part for operation authenticationSignin
   */
  static readonly AuthenticationSigninPath = '/api/1.0/authentication/signin';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `signin()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  signin$Response(params: {

        body: SigninRequestDto
    }): Observable<StrictHttpResponse<SigninResultDto>> {

    const rb = new RequestBuilder(this.rootUrl, AuthenticationService.AuthenticationSigninPath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<SigninResultDto>;
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

        body: SigninRequestDto
    }): Observable<SigninResultDto> {

      return this.signin$Response(params).pipe(
          map((r: StrictHttpResponse<SigninResultDto>) => r.body as SigninResultDto)
      );
  }

  /**
   * Path part for operation authenticationRefresh
   */
  static readonly AuthenticationRefreshPath = '/api/1.0/authentication/refresh';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `refresh()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  refresh$Response(params: {

        body: RefreshRequestDto
    }): Observable<StrictHttpResponse<RefreshResultDto>> {

    const rb = new RequestBuilder(this.rootUrl, AuthenticationService.AuthenticationRefreshPath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<RefreshResultDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `refresh$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  refresh(params: {

        body: RefreshRequestDto
    }): Observable<RefreshResultDto> {

      return this.refresh$Response(params).pipe(
          map((r: StrictHttpResponse<RefreshResultDto>) => r.body as RefreshResultDto)
      );
  }

  /**
   * Path part for operation authenticationSignout
   */
  static readonly AuthenticationSignoutPath = '/api/1.0/authentication/signout';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `signout()` instead.
   *
   * This method doesn't expect any request body.
   */
  signout$Response(params?: {

    }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, AuthenticationService.AuthenticationSignoutPath, 'post');
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
   * To access the full response (for headers, for example), `signout$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  signout(params?: {

    }): Observable<void> {

      return this.signout$Response(params).pipe(
          map((r: StrictHttpResponse<void>) => r.body as void)
      );
  }

  /**
   * Path part for operation authenticationRegisterAccount
   */
  static readonly AuthenticationRegisterAccountPath = '/api/1.0/authentication/register';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `registerAccount()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  registerAccount$Response(params: {

        body: RegisterRequestDto
    }): Observable<StrictHttpResponse<RegisterResultDto>> {

    const rb = new RequestBuilder(this.rootUrl, AuthenticationService.AuthenticationRegisterAccountPath, 'post');
    if (params) {


      rb.body(params.body, 'application/json');
    }
    return this.http.request(rb.build({
        responseType: 'json',
        accept: 'application/json'
    })).pipe(
        filter((r: any) => r instanceof HttpResponse),
        map((r: HttpResponse<any>) => {
            return r as StrictHttpResponse<RegisterResultDto>;
        })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `registerAccount$Response()` instead.
   *
   * This method sends `application/json` and handles request body of type `application/json`.
   */
  registerAccount(params: {

        body: RegisterRequestDto
    }): Observable<RegisterResultDto> {

      return this.registerAccount$Response(params).pipe(
          map((r: StrictHttpResponse<RegisterResultDto>) => r.body as RegisterResultDto)
      );
  }

}
