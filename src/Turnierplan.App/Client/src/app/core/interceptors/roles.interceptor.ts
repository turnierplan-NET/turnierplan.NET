import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthorizationService } from '../services/authorization.service';
import { Role } from '../../api';

@Injectable()
export class RolesInterceptor implements HttpInterceptor {
  constructor(private readonly authorizationService: AuthorizationService) {}

  public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      tap((response) => {
        if (response instanceof HttpResponse) {
          const headerValue = response.headers.get('X-Turnierplan-Roles');

          if (headerValue) {
            for (let entry of headerValue.split(',')) {
              const entryContents = entry.trim().split('=');
              this.authorizationService.addRolesToCache(
                entryContents[0],
                entryContents[1].split('+').map((x) => x as Role)
              );
            }
          }
        }
      })
    );
  }
}
