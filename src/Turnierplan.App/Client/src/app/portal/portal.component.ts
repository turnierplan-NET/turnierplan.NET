import { Component, Inject, OnDestroy, OnInit, DOCUMENT } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';

import { AuthenticatedUser } from '../core/models/identity';
import { AuthenticationService } from '../core/services/authentication.service';
import { DiscardChangesDetector, hasUnsavedChangesFunctionName } from '../core/guards/discard-changes.guard';
import { RouterLink, RouterOutlet } from '@angular/router';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { IsAdministratorDirective } from './directives/is-administrator.directive';
import { NgTemplateOutlet, NgClass } from '@angular/common';
import { FooterComponent } from '../core/components/footer/footer.component';
import { provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts/core';
import { BarChart } from 'echarts/charts';
import { GridComponent, TooltipComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';

echarts.use([BarChart, GridComponent, CanvasRenderer, TooltipComponent]);

type UserInfoAction = 'EditUserInfo' | 'ChangePassword' | 'Logout';

@Component({
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.scss'],
  imports: [
    RouterLink,
    TranslateDirective,
    NgbPopover,
    IsAdministratorDirective,
    NgTemplateOutlet,
    NgClass,
    RouterOutlet,
    FooterComponent,
    TranslatePipe
  ],
  providers: [provideEchartsCore({ echarts })]
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
