import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, switchMap, take, takeUntil } from 'rxjs';

import { NullableOfChangePasswordFailedReason } from '../../../api';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { NotificationService } from '../../../core/services/notification.service';

enum PasswordChangeFailedReason {
  EmptyOrExtraWhitespace = 'EmptyOrExtraWhitespace',
  PasswordsDoNotMatch = 'PasswordsDoNotMatch',
  InsecurePassword = 'InsecurePassword',
  InvalidCredentials = 'InvalidCredentials',
  NewPasswordEqualsCurrent = 'NewPasswordEqualsCurrent',
  UnexpectedError = 'UnexpectedError'
}

@Component({
  standalone: false,
  templateUrl: './change-password.component.html'
})
export class ChangePasswordComponent implements OnInit, OnDestroy {
  protected oldPassword: string = '';
  protected newPassword: string = '';
  protected confirmNewPassword: string = '';

  protected isLoading = false;
  protected failedReason?: PasswordChangeFailedReason;
  protected readonly history = history;

  private redirectTarget = '/portal';
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly authenticationService: AuthenticationService,
    private readonly notificationService: NotificationService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {}

  public ngOnInit(): void {
    this.route.queryParamMap.pipe(takeUntil(this.destroyed$)).subscribe((params) => {
      this.redirectTarget = params.get('redirect_to') ?? '/portal';
    });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  protected submit(): void {
    if (this.isLoading) {
      return;
    }

    this.isLoading = true;

    if (this.newPassword.trim().length !== this.newPassword.length || this.newPassword.length === 0) {
      setTimeout(() => {
        this.isLoading = false;
        this.failedReason = PasswordChangeFailedReason.EmptyOrExtraWhitespace;
      }, 500);

      return;
    }

    if (this.newPassword !== this.confirmNewPassword) {
      setTimeout(() => {
        this.isLoading = false;
        this.failedReason = PasswordChangeFailedReason.PasswordsDoNotMatch;
      }, 500);

      return;
    }

    this.authenticationService.authentication$
      .pipe(
        take(1),
        switchMap((user) => {
          return this.authenticationService.changePassword(user.emailAddress, this.newPassword, this.oldPassword);
        })
      )
      .subscribe((result) => {
        switch (result) {
          case 'success':
            this.notificationService.showNotification(
              'success',
              'Identity.ChangePassword.SuccessToast.Title',
              'Identity.ChangePassword.SuccessToast.Message'
            );

            void this.router.navigate([this.redirectTarget]);

            break;
          case NullableOfChangePasswordFailedReason.InsecurePassword:
            this.isLoading = false;
            this.failedReason = PasswordChangeFailedReason.InsecurePassword;
            break;
          case NullableOfChangePasswordFailedReason.InvalidCredentials:
            this.isLoading = false;
            this.failedReason = PasswordChangeFailedReason.InvalidCredentials;
            break;
          case NullableOfChangePasswordFailedReason.NewPasswordEqualsCurrent:
            this.isLoading = false;
            this.failedReason = PasswordChangeFailedReason.NewPasswordEqualsCurrent;
            break;
          default:
            this.isLoading = false;
            this.failedReason = PasswordChangeFailedReason.UnexpectedError;
            break;
        }
      });
  }
}
