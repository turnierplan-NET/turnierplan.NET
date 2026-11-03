import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, map, Observable, of, switchMap, tap } from 'rxjs';

import { TurnierplanApi } from '../../api/turnierplan-api';
import { me } from '../../api/fn/authentication/me';
import { MeEndpointResponse } from '../../api/models/me-endpoint-response';
import { logout } from '../../api/fn/authentication/logout';
import { login$XWwwFormUrlencoded } from '../../api/fn/authentication/login-x-www-form-urlencoded';
import { ChangePasswordFailedReason } from '../../api-temporary';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  public readonly authentication$ = new BehaviorSubject<MeEndpointResponse | undefined>(undefined);

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly router: Router
  ) {}

  public initialize(): void {
    this.turnierplanApi
      .invoke(me)
      .pipe(catchError(() => of(undefined)))
      .subscribe({
        next: (userInfo) => {
          if (userInfo) {
            this.authentication$.next(userInfo);
          } else {
            this.navigateToLoginPage(true);
          }
        }
      });
  }

  public login(userName: string, password: string): Observable<'success' | 'failure'> {
    return this.turnierplanApi.invoke(login$XWwwFormUrlencoded, { body: { username: userName, password: password } }).pipe(
      switchMap(() => this.turnierplanApi.invoke(me)),
      catchError(() => of(undefined)),
      tap((userInfo) => {
        if (userInfo) {
          this.authentication$.next(userInfo);
        }
      }),
      map((userInfo) => {
        return !!userInfo ? 'success' : 'failure';
      })
    );
  }

  public logout(): void {
    this.turnierplanApi
      .invoke(logout)
      .pipe(catchError(() => of(undefined)))
      .subscribe({
        next: () => {
          this.authentication$.next(undefined);
          this.navigateToLoginPage(false);
        }
      });
  }

  public isLoggedIn(): boolean {
    return !!this.authentication$.value;
  }

  public navigateToLoginPage(withBackRedirect: boolean): void {
    if (withBackRedirect) {
      void this.router.navigate(['/portal/login'], { queryParams: { redirect_to: this.router.url } });
    } else {
      void this.router.navigate(['/portal/login']);
    }
  }

  public navigateToChangePasswordPage(): void {
    void this.router.navigate(['portal/change-password'], { queryParams: { redirect_to: this.router.url } });
  }

  public checkIfUserIsAdministrator(): Observable<boolean> {
    return this.authentication$.pipe(map((info) => info?.isAdministrator === true));
  }

  public checkIfUserIsAllowedToCreateOrganization(): Observable<boolean> {
    return this.authentication$.pipe(map((info) => info?.canCreateOrganization === true));
  }

  public changePassword(
    userName: string,
    newPassword: string,
    currentPassword: string
  ): Observable<'success' | 'failure' | ChangePasswordFailedReason> {
    alert('Not implemented yet'); // TODO: Implement change password endpoint & logic
    return of();
  }
}
