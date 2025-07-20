import { Component, OnDestroy, OnInit } from '@angular/core';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { PlanningRealmDto, PlanningRealmsService } from '../../../api';
import { PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { Actions } from '../../../generated/actions';
import { of, Subject, switchMap, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { TitleService } from '../../services/title.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  standalone: false,
  templateUrl: './view-planning-realm.component.html'
})
export class ViewPlanningRealmComponent implements OnInit, OnDestroy {
  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected planningRealm?: PlanningRealmDto;

  protected currentPage = 0;
  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.ViewPlanningRealm.Pages.TournamentClasses',
      icon: 'bi-x-diamond'
    },
    {
      id: 1,
      title: 'Portal.ViewPlanningRealm.Pages.InvitationLinks',
      icon: 'bi-link-45deg'
    },
    {
      id: 2,
      title: 'Portal.ViewPlanningRealm.Pages.Applications',
      icon: 'bi-card-checklist'
    },
    {
      id: 3,
      title: 'Portal.ViewPlanningRealm.Pages.Settings',
      icon: 'bi-gear',
      authorization: Actions.GenericWrite
    }
  ];

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly planningRealmService: PlanningRealmsService,
    private readonly notificationService: NotificationService,
    private readonly titleService: TitleService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const planningRealmId = params.get('id');
          if (planningRealmId === null) {
            this.loadingState = { isLoading: false };
            return of();
          }
          this.loadingState = { isLoading: true };
          return this.planningRealmService.getPlanningRealm({ id: planningRealmId });
        })
      )
      .subscribe({
        next: (planningRealm) => {
          this.planningRealm = planningRealm;
          this.titleService.setTitleFrom(planningRealm);
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

  protected togglePage(number: number): void {
    this.currentPage = number;
  }

  protected deletePlanningRealm(): void {
    if (!this.planningRealm) {
      return;
    }

    const organizationId = this.planningRealm.organizationId;
    this.loadingState = { isLoading: true, error: undefined };
    this.planningRealmService.deletePlanningRealm({ id: this.planningRealm.id }).subscribe({
      next: () => {
        this.notificationService.showNotification(
          'info',
          'Portal.ViewPlanningRealm.DeleteWidget.SuccessToast.Title',
          'Portal.ViewPlanningRealm.DeleteWidget.SuccessToast.Message'
        );
        void this.router.navigate([`../../organization/${organizationId}`], { relativeTo: this.route });
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }
}
