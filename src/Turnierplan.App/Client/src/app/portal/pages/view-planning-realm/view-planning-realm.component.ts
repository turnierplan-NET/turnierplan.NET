import { Component, OnDestroy, OnInit } from '@angular/core';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { InvitationLinksService, PlanningRealmDto, PlanningRealmsService, TournamentClassesService } from '../../../api';
import { PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { Actions } from '../../../generated/actions';
import { Observable, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { TitleService } from '../../services/title.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TextInputDialogComponent } from '../../components/text-input-dialog/text-input-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';

@Component({
  standalone: false,
  templateUrl: './view-planning-realm.component.html'
})
export class ViewPlanningRealmComponent implements OnInit, OnDestroy {
  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected planningRealm?: PlanningRealmDto;

  protected isUpdatingName = false;

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
    private readonly tournamentClassService: TournamentClassesService,
    private readonly invitationLinkService: InvitationLinksService,
    private readonly notificationService: NotificationService,
    private readonly titleService: TitleService,
    private readonly modalService: NgbModal
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

  protected addTournamentClass(): void {
    if (!this.planningRealm) {
      return;
    }

    const planningRealmId = this.planningRealm.id;

    this.openModalForEnteringName('NewTournamentClass')
      .pipe(
        tap(() => (this.loadingState = { isLoading: true })),
        switchMap((name) => this.tournamentClassService.createTournamentClass({ id: planningRealmId, body: { name: name } })),
        switchMap(() => this.planningRealmService.getPlanningRealm({ id: planningRealmId }))
      )
      .subscribe({
        next: (planningRealm) => {
          this.planningRealm = planningRealm;
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected addInvitationLink(): void {
    if (!this.planningRealm) {
      return;
    }

    const planningRealmId = this.planningRealm.id;

    this.openModalForEnteringName('NewInvitationLink')
      .pipe(
        tap(() => (this.loadingState = { isLoading: true })),
        switchMap((name) => this.invitationLinkService.createInvitationLink({ id: planningRealmId, body: { name: name } })),
        switchMap(() => this.planningRealmService.getPlanningRealm({ id: planningRealmId }))
      )
      .subscribe({
        next: (planningRealm) => {
          this.planningRealm = planningRealm;
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected renamePlanningRealm(name: string): void {
    if (!this.planningRealm || name === this.planningRealm.name || this.isUpdatingName) {
      return;
    }

    this.isUpdatingName = true;

    this.planningRealmService.setPlanningRealmName({ id: this.planningRealm.id, body: { name: name } }).subscribe({
      next: () => {
        if (this.planningRealm) {
          this.planningRealm.name = name;
          this.titleService.setTitleFrom(this.planningRealm);
        }
        this.isUpdatingName = false;
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
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

  private openModalForEnteringName(key: string): Observable<string> {
    const ref = this.modalService.open(TextInputDialogComponent, {
      centered: true,
      size: 'md',
      fullscreen: 'md'
    });

    const component = ref.componentInstance as TextInputDialogComponent;
    component.init(`Portal.ViewPlanningRealm.${key}`, '', false, true);

    return ref.closed.pipe(map((x) => x as string));
  }
}
