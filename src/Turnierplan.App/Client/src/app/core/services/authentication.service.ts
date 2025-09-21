import { Injectable, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { catchError, map, Observable, of, ReplaySubject, Subject, switchMap, tap } from 'rxjs';

import { AuthenticatedUser } from '../models/identity';

interface TurnierplanAccessToken {
  exp: number;
  mail: string;
  name: string;
  adm?: string;
  uid: string;
}

interface TurnierplanRefreshToken {
  exp: number;
  uid: string;
}

@Injectable({ providedIn: 'root' })
export class AuthenticationService implements OnDestroy {
  private static readonly localStorageUserIdKey = 'tp_id_userId';
  private static readonly localStorageUserNameKey = 'tp_id_userName';
  private static readonly localStorageUserEMailKey = 'tp_id_userEMail';
  private static readonly localStorageUserAdministratorKey = 'tp_id_userAdmin';
  private static readonly localStorageAccessTokenExpiryKey = 'tp_id_accTokenExp';
  private static readonly localStorageRefreshTokenExpiryKey = 'tp_id_rfsTokenExp';

  // Clock skew should be as short as possible, but still long enough that a request to the token
  // refresh endpoint can complete even in the case of a bad connection or unfavorable conditions.
  private static readonly tokenExpiryCheckClockSkewSeconds = 10;

  public readonly authentication$ = new ReplaySubject<AuthenticatedUser>(1);

  private isEnsuringAccessToken = false;
  private tokenEnsureCompleted$?: Subject<boolean>;
  private readonly destroyed$ = new Subject<void>();

  constructor(private readonly router: Router) {
    const storedUserId = this.readUserIdFromLocalStorage();
    const storedUserName = this.readUserNameFromLocalStorage();
    const storedUserEMail = this.readUserEMailFromLocalStorage();

    if (storedUserId && storedUserName && storedUserEMail) {
      this.authentication$.next({ id: storedUserId, displayName: storedUserName, emailAddress: storedUserEMail });
    }
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  public login(email: string, password: string): Observable<'success' | 'failure'> {
    return this.turnierplanApi.invoke(login, { body: { eMail: email, password: password } }).pipe(
      catchError(() => of(undefined)),
      map((result) => {
        if (result && result.success && result.accessToken && result.refreshToken) {
          const decodedAccessToken = this.decodeAccessToken(result.accessToken);
          const decodedRefreshToken = this.decodeRefreshToken(result.refreshToken);

          this.updateLocalStorageCache(
            decodedAccessToken.uid,
            decodedAccessToken.name,
            decodedAccessToken.mail,
            decodedAccessToken.adm === 'true',
            decodedAccessToken.exp,
            decodedRefreshToken.exp
          );

          this.authentication$.next({
            id: decodedAccessToken.uid,
            displayName: decodedAccessToken.name,
            emailAddress: decodedAccessToken.mail
          });

          return 'success';
        } else {
          return 'failure';
        }
      })
    );
  }

  public logout(): void {
    this.logoutAndClearData(() => window.location.assign('/')).subscribe();
  }

  public openEditUserInfoForm(): void {
    void this.router.navigate(['portal/user-info'], { queryParams: { redirect_to: this.router.url } });
  }

  public openChangePasswordForm(): void {
    void this.router.navigate(['portal/change-password'], { queryParams: { redirect_to: this.router.url } });
  }

  public ensureAccessToken(): Observable<boolean> {
    if (this.isEnsuringAccessToken) {
      return this.tokenEnsureCompleted$!.asObservable();
    }

    this.isEnsuringAccessToken = true;
    this.tokenEnsureCompleted$ = new Subject<boolean>();

    return this.ensureAccessTokenUnprotected().pipe(
      tap((status) => {
        this.tokenEnsureCompleted$?.next(status);
        this.tokenEnsureCompleted$?.complete();
        this.isEnsuringAccessToken = false;
      })
    );
  }

  public isLoggedIn(): boolean {
    // User is logged in if he either has a valid access token or, alternatively, a valid refresh token
    return !this.isAccessTokenExpired() || !this.isRefreshTokenExpired();
  }

  public checkIfUserIsAdministrator(): Observable<boolean> {
    return this.authentication$.pipe(map(() => localStorage.getItem(AuthenticationService.localStorageUserAdministratorKey) === 'true'));
  }

  public changePassword(
    userEmail: string,
    newPassword: string,
    currentPassword: string
  ): Observable<'success' | 'failure' | NullableOfChangePasswordFailedReason> {
    return this.turnierplanApi
      .invoke(changePassword, {
        body: {
          eMail: userEmail,
          newPassword: newPassword,
          currentPassword: currentPassword
        }
      })
      .pipe(
        catchError(() => of(undefined)),
        map((result) => {
          if (result?.success === true) {
            if (result.refreshToken) {
              const decodedRefreshToken = this.decodeRefreshToken(result.refreshToken);
              this.updateLocalStorageCacheRefreshTokenExpiryOnly(decodedRefreshToken.exp);
            }

            return 'success';
          } else if (result?.reason) {
            return result.reason;
          } else {
            return 'failure';
          }
        })
      );
  }

  public changeUserInformation(userName: string, emailAddress: string): Observable<'success' | 'emailVerificationPending' | 'failure'> {
    return this.turnierplanApi.invoke(updateUserData, { body: { userName: userName, eMail: emailAddress } }).pipe(
      catchError(() => of(undefined)),
      map((result) => {
        if (result?.success !== true) {
          return 'failure';
        }

        this.updateLocalStorageCacheUserInformationOnly(userName, emailAddress);
        this.authentication$.next({ id: this.readUserIdFromLocalStorage()!, displayName: userName, emailAddress: emailAddress });

        return 'success';
      })
    );
  }

  private ensureAccessTokenUnprotected(): Observable<boolean> {
    const accessTokenExpiry = this.readAccessTokenExpiryFromLocalStorage();

    if (accessTokenExpiry === undefined) {
      return this.logoutAndClearData(() => {
        void this.router.navigate(['/portal/login']);
      }).pipe(map(() => false));
    }

    const logoutWithRedirect = (): Observable<boolean> => {
      return this.logoutAndClearData(() => {
        void this.router.navigate(['/portal/login'], {
          queryParams: { redirect_to: this.router.url, email: this.readUserEMailFromLocalStorage() }
        });
      }).pipe(map(() => false));
    };

    // Note: Expiry check includes some clock skew
    const shouldRefresh = this.isAccessTokenExpired();

    if (shouldRefresh) {
      if (this.isRefreshTokenExpired()) {
        return logoutWithRedirect();
      }

      return this.turnierplanApi.invoke(refresh).pipe(
        switchMap((result) => {
          if (result.success && result.accessToken && result.refreshToken) {
            const decodedAccessToken = this.decodeAccessToken(result.accessToken);
            const decodedRefreshToken = this.decodeRefreshToken(result.refreshToken);

            this.updateLocalStorageCache(
              decodedAccessToken.uid,
              decodedAccessToken.name,
              decodedAccessToken.mail,
              decodedAccessToken.adm === 'true',
              decodedAccessToken.exp,
              decodedRefreshToken.exp
            );

            this.authentication$.next({
              id: decodedAccessToken.uid,
              displayName: decodedAccessToken.name,
              emailAddress: decodedAccessToken.mail
            });

            return of(true);
          } else {
            return logoutWithRedirect();
          }
        }),
        catchError(() => {
          return logoutWithRedirect();
        })
      );
    } else {
      return of(true);
    }
  }

  private isAccessTokenExpired(): boolean {
    const expiry = this.readAccessTokenExpiryFromLocalStorage();

    // Add some clock skew to prevent race condition
    return expiry === undefined || expiry * 1000 < new Date().getTime() + AuthenticationService.tokenExpiryCheckClockSkewSeconds * 1000;
  }

  private isRefreshTokenExpired(): boolean {
    const expiry = this.readRefreshTokenExpiryFromLocalStorage();

    // Add some clock skew to prevent race condition
    return expiry === undefined || expiry * 1000 < new Date().getTime() + AuthenticationService.tokenExpiryCheckClockSkewSeconds * 1000;
  }

  private decodeAccessToken(token: string): TurnierplanAccessToken {
    return jwtDecode<TurnierplanAccessToken>(token);
  }

  private decodeRefreshToken(token: string): TurnierplanRefreshToken {
    return jwtDecode<TurnierplanRefreshToken>(token);
  }

  private readUserIdFromLocalStorage(): string | undefined {
    return localStorage.getItem(AuthenticationService.localStorageUserIdKey) ?? undefined;
  }

  private readUserNameFromLocalStorage(): string | undefined {
    return localStorage.getItem(AuthenticationService.localStorageUserNameKey) ?? undefined;
  }

  private readUserEMailFromLocalStorage(): string | undefined {
    return localStorage.getItem(AuthenticationService.localStorageUserEMailKey) ?? undefined;
  }

  private readAccessTokenExpiryFromLocalStorage(): number | undefined {
    const accessTokenExpiry = localStorage.getItem(AuthenticationService.localStorageAccessTokenExpiryKey);
    if (!accessTokenExpiry) {
      return undefined;
    }
    return +accessTokenExpiry;
  }

  private readRefreshTokenExpiryFromLocalStorage(): number | undefined {
    const refreshTokenExpiry = localStorage.getItem(AuthenticationService.localStorageRefreshTokenExpiryKey);
    if (!refreshTokenExpiry) {
      return undefined;
    }
    return +refreshTokenExpiry;
  }

  private updateLocalStorageCache(
    userId: string,
    userName: string,
    userEMail: string,
    userIsAdmin: boolean,
    accessTokenExpiry: number,
    refreshTokenExpiry: number
  ): void {
    localStorage.setItem(AuthenticationService.localStorageUserIdKey, userId);
    localStorage.setItem(AuthenticationService.localStorageUserNameKey, userName);
    localStorage.setItem(AuthenticationService.localStorageUserEMailKey, userEMail);
    localStorage.setItem(AuthenticationService.localStorageUserAdministratorKey, `${userIsAdmin}`);

    localStorage.setItem(AuthenticationService.localStorageAccessTokenExpiryKey, `${accessTokenExpiry}`);
    localStorage.setItem(AuthenticationService.localStorageRefreshTokenExpiryKey, `${refreshTokenExpiry}`);
  }

  private updateLocalStorageCacheUserInformationOnly(userName: string, userEMail: string): void {
    localStorage.setItem(AuthenticationService.localStorageUserNameKey, userName);
    localStorage.setItem(AuthenticationService.localStorageUserEMailKey, userEMail);
  }

  private updateLocalStorageCacheRefreshTokenExpiryOnly(refreshTokenExpiry: number): void {
    localStorage.setItem(AuthenticationService.localStorageRefreshTokenExpiryKey, `${refreshTokenExpiry}`);
  }

  private logoutAndClearData(navigateTo?: () => void): Observable<void> {
    const logout$ = this.turnierplanApi.invoke(logout).pipe(
      catchError(() => of(undefined)),
      tap(() => {
        localStorage.removeItem(AuthenticationService.localStorageUserNameKey);
        localStorage.removeItem(AuthenticationService.localStorageUserEMailKey);
        localStorage.removeItem(AuthenticationService.localStorageAccessTokenExpiryKey);
        localStorage.removeItem(AuthenticationService.localStorageRefreshTokenExpiryKey);
      }),
      map(() => void 0)
    );

    if (navigateTo) {
      return logout$.pipe(
        tap(() => {
          navigateTo();
        })
      );
    } else {
      return logout$;
    }
  }
}
