import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { from, switchMap } from 'rxjs';

import { OrganizationsService } from '../../../api';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';

@Component({
  templateUrl: './create-organization.component.html'
})
export class CreateOrganizationComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: false };
  protected organizationName = new FormControl('', { nonNullable: true });

  constructor(
    private readonly organizationService: OrganizationsService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly titleService: TitleService
  ) {}

  public ngOnInit(): void {
    this.titleService.setTitleTranslated('Portal.CreateOrganization.Title');
  }

  protected confirmButtonClicked(): void {
    if (this.organizationName.valid && !this.loadingState.isLoading) {
      this.loadingState = { isLoading: true };
      this.organizationService
        .createOrganization({ body: { name: this.organizationName.value } })
        .pipe(switchMap((organization) => from(this.router.navigate(['../../organization', organization.id], { relativeTo: this.route }))))
        .subscribe({
          next: () => {
            this.loadingState = { isLoading: false };
          },
          error: (error) => {
            this.loadingState = { isLoading: false, error: error };
          }
        });
    }
  }
}
