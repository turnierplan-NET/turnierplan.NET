import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';

import { AuthenticatedUser } from '../core/models/identity';
import { AuthenticationService } from '../core/services/authentication.service';

import { RoleIds } from './helpers/role-ids';

type UserInfoAction = 'EditUserInfo' | 'ChangePassword' | 'Logout';

@Component({
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.scss']
})
export class PortalComponent implements OnInit, OnDestroy {
  protected readonly administratorRoleId = RoleIds.administratorRoleId;

  protected currentUser?: AuthenticatedUser;
  protected footerStyle = '';

  private readonly destroyed$ = new Subject<void>();

  constructor(private readonly authenticationService: AuthenticationService, @Inject(DOCUMENT) private readonly document: Document) {}

  public ngOnInit(): void {
    this.authenticationService.authentication$.pipe(takeUntil(this.destroyed$)).subscribe((user) => {
      this.currentUser = user;
    });

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
