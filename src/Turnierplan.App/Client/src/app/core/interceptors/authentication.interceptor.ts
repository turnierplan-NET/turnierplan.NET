import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';

import { AuthenticationService } from '../services/authentication.service';

const doesPathRequireAuthentication = (path: string): boolean => {
  return /^\/api(?!\/identity)/.test(path);
};

export const authenticationInterceptor = (request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  if (!request.url.startsWith(window.origin)) {
    return next(request);
  }

  if (!doesPathRequireAuthentication(request.url.substring(window.origin.length))) {
    return next(request);
  }

  return inject(AuthenticationService)
    .ensureAccessToken()
    .pipe(
      switchMap((shouldContinue) => {
        if (shouldContinue) {
          return next(request);
        }

        return of();
      })
    );
};
