import { Component, OnInit } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { from, switchMap } from 'rxjs';

import { OrganizationsService } from '../../../api';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';

@Component({
  templateUrl: './create-organization.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslateDirective,
    FormsModule,
    ReactiveFormsModule,
    NgClass,
    ActionButtonComponent,
    TranslatePipe
  ]
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
