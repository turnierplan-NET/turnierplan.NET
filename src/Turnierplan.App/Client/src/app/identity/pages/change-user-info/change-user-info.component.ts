import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, take, takeUntil } from 'rxjs';

import { AuthenticationService } from '../../../core/services/authentication.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  templateUrl: './change-user-info.component.html'
})
export class ChangeUserInfoComponent implements OnInit, OnDestroy {
  protected previousEmailAddress: string = '';

  protected userName: string = '';
  protected emailAddress: string = '';

  protected isLoading = false;
  protected isRequestFailed = false;
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

    this.authenticationService.authentication$.pipe(take(1)).subscribe((result) => {
      this.userName = result.displayName;
      this.previousEmailAddress = result.emailAddress;
      this.emailAddress = result.emailAddress;
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

    this.authenticationService.changeUserInformation(this.userName, this.emailAddress).subscribe({
      next: (result) => {
        switch (result) {
          case 'success':
            this.notificationService.showNotification(
              'success',
              'Identity.ChangeUserInfo.SuccessToast.Title',
              'Identity.ChangeUserInfo.SuccessToast.Message'
            );

            break;
          case 'emailVerificationPending':
            this.notificationService.showNotification(
              'success',
              'Identity.ChangeUserInfo.EmailVerificationPendingToast.Title',
              'Identity.ChangeUserInfo.EmailVerificationPendingToast.Message',
              60000
            );

            break;
          case 'failure':
            this.isRequestFailed = true;
            this.isLoading = false;

            return;
        }

        void this.router.navigate([this.redirectTarget]);
      }
    });
  }
}
