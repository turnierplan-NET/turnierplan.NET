import { HttpEvent, HttpEventType, HttpHandlerFn, HttpHeaderResponse, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable, of, switchMap, tap } from 'rxjs';

import { AuthenticationService } from '../services/authentication.service';

export const doesPathRequireAuthentication = (path: string): boolean => {
  return /^\/api\//.test(path);
};

export const authenticationInterceptor = (request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  if (!doesPathRequireAuthentication(request.url)) {
    return next(request);
  }

  const authenticationService = inject(AuthenticationService);

  return next(request).pipe(
    tap((httpEvent) => {
      if (httpEvent.type === HttpEventType.ResponseHeader) {
        const r = httpEvent as HttpHeaderResponse;
        if (r.status === 401) {
          authenticationService.navigateToLoginPage(true);
        }
      }
    })
  );
};
