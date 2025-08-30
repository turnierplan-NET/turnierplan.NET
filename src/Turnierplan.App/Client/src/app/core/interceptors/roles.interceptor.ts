import { inject } from '@angular/core';
import { HttpEvent, HttpHandlerFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthorizationService } from '../services/authorization.service';
import { Role } from '../../api';

export const rolesInterceptor = (request: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const authorizationService = inject(AuthorizationService);

  return next(request).pipe(
    tap((response) => {
      if (response instanceof HttpResponse) {
        const headerValue = response.headers.get('X-Turnierplan-Roles');

        if (headerValue) {
          for (let entry of headerValue.split(',')) {
            const entryContents = entry.trim().split('=');
            authorizationService.addRolesToCache(
              entryContents[0],
              entryContents[1].split('+').map((x) => x as Role)
            );
          }
        }
      }
    })
  );
};
