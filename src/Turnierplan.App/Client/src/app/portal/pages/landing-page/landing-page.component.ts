import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';

import { OrganizationDto, OrganizationsService } from '../../../api';
import { PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { RoleIds } from '../../helpers/role-ids';
import { TitleService } from '../../services/title.service';

@Component({
  standalone: false,
  templateUrl: './landing-page.component.html'
})
export class LandingPageComponent implements OnInit {
  protected readonly administratorRoleId = RoleIds.administratorRoleId;

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
