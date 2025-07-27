import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';

import { AuthenticatedUser } from '../core/models/identity';
import { AuthenticationService } from '../core/services/authentication.service';
import { DiscardChangesDetector, hasUnsavedChangesFunctionName } from '../core/guards/discard-changes.guard';

type UserInfoAction = 'EditUserInfo' | 'ChangePassword' | 'Logout';

@Component({
  standalone: false,
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.scss']
})
export class PortalComponent implements OnInit, OnDestroy {
  protected currentComponent?: Object;
  protected currentUser?: AuthenticatedUser;
  protected footerStyle = '';

  private readonly destroyed$ = new Subject<void>();

  private readonly beforeUnloadListener = (event: BeforeUnloadEvent) => {
    if (this.currentComponent && hasUnsavedChangesFunctionName in this.currentComponent) {
      const hasUnsavedChanges = (this.currentComponent as DiscardChangesDetector).hasUnsavedChanges();
      if (hasUnsavedChanges) {
        event.preventDefault();
      }
    }
  };

  constructor(
    private readonly authenticationService: AuthenticationService,
    @Inject(DOCUMENT) private readonly document: Document
  ) {}

  public ngOnInit(): void {
    this.authenticationService.authentication$.pipe(takeUntil(this.destroyed$)).subscribe((user) => {
      this.currentUser = user;
    });

    window.addEventListener('beforeunload', this.beforeUnloadListener);

    // Listen for changes to the style attribute of the <body> caused by modals appearing/disappearing to compensate for
    // the loss of the scroll bar when a modal is open. The <body> style change need to be manually applied to the footer
    // to prevent "jumping" when opening/closing modals.
    new MutationObserver(() => {
      this.footerStyle = `padding-right: ${this.document.body.style.paddingRight}`;
    }).observe(this.document.body, { attributes: true, attributeFilter: ['style'] });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();

    window.removeEventListener('beforeunload', this.beforeUnloadListener);
  }

  protected getUserInfoActionIcon(action: UserInfoAction): string {
    switch (action) {
      case 'EditUserInfo':
        return 'bi-person-vcard';
      case 'ChangePassword':
        return 'bi-key';
      case 'Logout':
        return 'bi-door-open';
    }
  }

  protected userInfoActionClicked(action: UserInfoAction): void {
    switch (action) {
      case 'EditUserInfo':
        this.authenticationService.openEditUserInfoForm();
        break;
      case 'ChangePassword':
        this.authenticationService.openChangePasswordForm();
        break;
      case 'Logout':
        this.authenticationService.logout();
        break;
    }
  }
}
