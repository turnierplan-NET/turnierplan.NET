import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { IsAdministratorDirective } from '../../directives/is-administrator.directive';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { RouterLink } from '@angular/router';
import { IllustrationComponent } from '../../components/illustration/illustration.component';
import { TranslateDirective } from '@ngx-translate/core';
import { BadgeComponent } from '../../components/badge/badge.component';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { UpdatesCheckComponent } from '../../components/updates-check/updates-check.component';
import { OrganizationDto } from '../../../api/models/organization-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getOrganizations } from '../../../api/fn/organizations/get-organizations';
import { AllowCreateOrganizationDirective } from '../../directives/allow-create-organization.directive';

@Component({
  templateUrl: './landing-page.component.html',
  imports: [
    LoadingStateDirective,
    IsAdministratorDirective,
    ActionButtonComponent,
    RouterLink,
    IllustrationComponent,
    TranslateDirective,
    BadgeComponent,
    E2eDirective,
    UpdatesCheckComponent,
    AllowCreateOrganizationDirective
  ]
})
export class LandingPageComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: true };
  protected organizations: OrganizationDto[] = [];

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly titleService: TitleService
  ) {}

  public ngOnInit(): void {
    this.titleService.setTitleTranslated('Portal.LandingPage.Title');

    this.turnierplanApi
      .invoke(getOrganizations)
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
