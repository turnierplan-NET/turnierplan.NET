import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { from, of, Subject, switchMap, takeUntil } from 'rxjs';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';

@Component({
  templateUrl: './create-venue.component.html',
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
export class CreateVenueComponent implements OnDestroy {
  protected loadingState: LoadingState = { isLoading: false };

  protected organization?: OrganizationDto;
  protected venueName = new FormControl('', { nonNullable: true });

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly titleService: TitleService
  ) {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const organizationId = params.get('id');
          if (organizationId === null) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }
          this.loadingState = { isLoading: true };
          return this.organizationService.getOrganization({ id: organizationId });
        })
      )
      .subscribe({
        next: (result) => {
          this.organization = result;
          this.titleService.setTitleFrom(result);
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  protected confirmButtonClicked(): void {
    if (this.venueName.valid && !this.loadingState.isLoading && this.organization) {
      this.loadingState = { isLoading: true };
      this.venueService
        .createVenue({
          body: {
            organizationId: this.organization.id,
            name: this.venueName.value
          }
        })
        .pipe(switchMap((venue) => from(this.router.navigate(['../../../../venue/', venue.id], { relativeTo: this.route }))))
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
