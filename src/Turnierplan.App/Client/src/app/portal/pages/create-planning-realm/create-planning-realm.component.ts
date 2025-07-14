import { Component, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { from, of, Subject, switchMap, takeUntil } from 'rxjs';

import { OrganizationDto, OrganizationsService, PlanningRealmsService } from '../../../api';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';

@Component({
  standalone: false,
  templateUrl: './create-planning-realm.component.html'
})
export class CreatePlanningRealmComponent implements OnDestroy {
  protected loadingState: LoadingState = { isLoading: false };

  protected organization?: OrganizationDto;
  protected planningRealmName = new FormControl('', { nonNullable: true });

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly organizationService: OrganizationsService,
    private readonly planningRealmService: PlanningRealmsService,
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
    if (this.planningRealmName.valid && !this.loadingState.isLoading && this.organization) {
      this.loadingState = { isLoading: true };
      this.planningRealmService
        .createPlanningRealm({
          body: {
            organizationId: this.organization.id,
            name: this.planningRealmName.value
          }
        })
        .pipe(
          switchMap((planningRealm) =>
            from(this.router.navigate(['../../../../planning-realm/', planningRealm.id], { relativeTo: this.route }))
          )
        )
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
