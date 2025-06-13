import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';

import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {
  constructor(private readonly authenticationService: AuthenticationService) {}

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (!request.url.startsWith(window.origin)) {
      return next.handle(request);
    }

    if (!AuthenticationInterceptor.doesPathRequireAuthentication(request.url.substring(window.origin.length))) {
      return next.handle(request);
    }

    return this.authenticationService.ensureAccessToken().pipe(
      switchMap((shouldContinue) => {
        if (shouldContinue) {
          return next.handle(request);
        }

        return of();
      })
    );
  }

  public static doesPathRequireAuthentication(path: string): boolean {
    return /^\/api(?!\/identity)/.test(path);
  }
}
