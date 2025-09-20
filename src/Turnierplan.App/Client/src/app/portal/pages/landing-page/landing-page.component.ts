import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';

import { OrganizationDto, OrganizationsService } from '../../../api';
import { PageFrameNavigationTab, PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { IsAdministratorDirective } from '../../directives/is-administrator.directive';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { RouterLink } from '@angular/router';
import { AlertComponent } from '../../components/alert/alert.component';
import { IllustrationComponent } from '../../components/illustration/illustration.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { BadgeComponent } from '../../components/badge/badge.component';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { UpdatesCheckComponent } from '../../components/updates-check/updates-check.component';

@Component({
  templateUrl: './landing-page.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    IsAdministratorDirective,
    ActionButtonComponent,
    RouterLink,
    AlertComponent,
    IllustrationComponent,
    TranslateDirective,
    BadgeComponent,
    TranslatePipe,
    E2eDirective,
    UpdatesCheckComponent
  ]
})
export class LandingPageComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: true };
  protected organizations: OrganizationDto[] = [];

  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.LandingPage.Pages.Organizations',
      icon: 'bi-boxes'
    }
  ];

  constructor(
    private readonly organizationService: OrganizationsService,
    private readonly titleService: TitleService
  ) {}

  public ngOnInit(): void {
    this.titleService.setTitleTranslated('Portal.LandingPage.Title');

    this.organizationService
      .getOrganizations()
      .pipe(take(1))
      .subscribe({
        next: (result) => {
          this.organizations = result;
          this.organizations.sort((a, b) => a.name.localeCompare(b.name));
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }
}
