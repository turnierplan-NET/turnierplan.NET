import { Component, OnDestroy, OnInit } from '@angular/core';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { PlanningRealmDto, PlanningRealmsService, UpdatePlanningRealmEndpointRequest } from '../../../api';
import { PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { Actions } from '../../../generated/actions';
import { Observable, of, Subject, switchMap, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { TitleService } from '../../services/title.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TextInputDialogComponent } from '../../components/text-input-dialog/text-input-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { DiscardChangesDetector } from '../../../core/guards/discard-changes.guard';

export type UpdatePlanningRealmFunc = (modifyFunc: (planningRealm: PlanningRealmDto) => boolean) => void;

@Component({
  standalone: false,
  templateUrl: './view-planning-realm.component.html'
})
export class ViewPlanningRealmComponent implements OnInit, OnDestroy, DiscardChangesDetector {
  private static DefaultInvitationLinkColorCodes: string[] = [
    'ff9900',
    'ff5500',
    'bb5500',
    '55bb00',
    'ff0055',
    'bb0055',
    '5500ff',
    '9900bb',
    '00bb99',
    '0099ff',
    '0099bb',
    '0055bb'
  ];

  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected updateFunction: UpdatePlanningRealmFunc;
  protected planningRealm?: PlanningRealmDto;
  protected _hasUnsavedChanges = false;

  // When creating tournament classes or invitation links, we use negative IDs until saving
  // the changes to the backend. When saving, all negative IDs are replaced with 'null',
  // prompting the backend to create new objects instead of replacing existing ones.
  protected nextId = -1;

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
  private originalPlanningRealm?: PlanningRealmDto;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly planningRealmService: PlanningRealmsService,
    private readonly notificationService: NotificationService,
    private readonly titleService: TitleService,
    private readonly modalService: NgbModal
  ) {
    this.updateFunction = (modifyFunc: (planningRealm: PlanningRealmDto) => boolean) => {
      if (this.planningRealm) {
        if (modifyFunc(this.planningRealm)) {
          this._hasUnsavedChanges = true;
        }
      }
    };
  }

  protected get isApplicationsPage(): boolean {
    return this.currentPage === 2;
  }

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
          this.setPlanningRealm(planningRealm);
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

  public hasUnsavedChanges(): boolean {
    return this._hasUnsavedChanges;
  }

  protected togglePage(number: number): void {
    this.currentPage = number;
  }

  protected addTournamentClass(): void {
    this.openModalForEnteringName('NewTournamentClass').subscribe({
      next: (name) => {
        this.updateFunction((planningRealm) => {
          planningRealm.tournamentClasses.push({
            id: this.nextId--,
            name: name.trim(),
            maxTeamCount: null,
            numberOfTeams: 0
          });

          return true;
        });
      }
    });
  }

  protected addInvitationLink(): void {
    this.openModalForEnteringName('NewInvitationLink').subscribe({
      next: (name) => {
        this.updateFunction((planningRealm) => {
          planningRealm.invitationLinks.push({
            colorCode: this.getColorCode(),
            contactEmail: null,
            contactPerson: null,
            contactTelephone: null,
            description: null,
            entries: [],
            id: this.nextId--,
            name: name.trim(),
            numberOfApplications: 0,
            primaryLogo: null,
            publicId: '',
            secondaryLogo: null,
            title: null,
            validUntil: null
          });

          return true;
        });
      }
    });
  }

  protected renamePlanningRealm(name: string): void {
    if (!this.planningRealm) {
      return;
    }

    this.updateFunction((planningRealm) => {
      planningRealm.name = name;
      return true;
    });

    this.titleService.setTitleFrom(this.planningRealm);
  }

  protected savePlanningRealm(): void {
    if (!this.planningRealm || this.loadingState.isLoading) {
      return;
    }

    const planningRealmId = this.planningRealm.id;
    this.loadingState = { isLoading: true };

    const request: UpdatePlanningRealmEndpointRequest = {
      name: this.planningRealm.name,
      tournamentClasses: this.planningRealm.tournamentClasses.map((x) => ({
        id: x.id < 0 ? null : x.id,
        name: x.name,
        maxTeamCount: x.maxTeamCount === null ? undefined : x.maxTeamCount
      })),
      invitationLinks: this.planningRealm.invitationLinks.map((x) => ({
        id: x.id < 0 ? null : x.id,
        name: x.name,
        colorCode: x.colorCode,
        contactEmail: x.contactEmail,
        contactPerson: x.contactPerson,
        contactTelephone: x.contactTelephone,
        description: x.description,
        primaryLogoId: x.primaryLogo?.id,
        secondaryLogoId: x.secondaryLogo?.id,
        title: x.title,
        validUntil: x.validUntil
      }))
    };

    this.planningRealmService
      .updatePlanningRealm({ id: planningRealmId, body: request })
      .pipe(switchMap(() => this.planningRealmService.getPlanningRealm({ id: planningRealmId })))
      .subscribe({
        next: (planningRealm) => {
          this.setPlanningRealm(planningRealm);
          this.loadingState = { isLoading: false };
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

  private setPlanningRealm(planningRealm: PlanningRealmDto): void {
    this.originalPlanningRealm = planningRealm;

    // Create a working copy which can be modified and saved/discarded
    this.planningRealm = JSON.parse(JSON.stringify(this.originalPlanningRealm)) as PlanningRealmDto;
    this._hasUnsavedChanges = false;

    this.titleService.setTitleFrom(this.planningRealm);
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

  private getColorCode(): string {
    if (!this.planningRealm) {
      return 'aaaaaa';
    }

    const planningRealm = this.planningRealm;
    let availableColorCodes = ViewPlanningRealmComponent.DefaultInvitationLinkColorCodes.filter(
      (color) => !planningRealm.invitationLinks.some((x) => x.colorCode === color)
    );

    if (availableColorCodes.length === 0) {
      availableColorCodes = ViewPlanningRealmComponent.DefaultInvitationLinkColorCodes;
    }

    return availableColorCodes[Math.floor(Math.random() * availableColorCodes.length)];
  }
}
