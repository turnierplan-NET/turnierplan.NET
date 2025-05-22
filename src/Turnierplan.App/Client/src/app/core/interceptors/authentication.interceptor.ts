import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of, switchMap } from 'rxjs';

import { AuthenticationService } from '../services/authentication.service';

@Injectable()
export class AuthenticationInterceptor implements HttpInterceptor {
  private readonly apiRoutesPrefix = `${window.origin}/api`;
  private readonly identityRoutesPrefix = `${window.origin}/api/identity`;

  constructor(private readonly authenticationService: AuthenticationService) {}

  public intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // IDEA: The check below could maybe be improved with a regex. Also, the API route for changing the user data also
    //       requires authentication even though its path '/api/identity/user-data/' is excluded by the logic below.
    const requireAuthentication = request.url.startsWith(this.apiRoutesPrefix) && !request.url.startsWith(this.identityRoutesPrefix);

    if (!requireAuthentication) {
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
}
