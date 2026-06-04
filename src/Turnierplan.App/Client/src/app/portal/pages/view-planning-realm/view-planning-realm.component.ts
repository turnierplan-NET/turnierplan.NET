import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { PageFrameComponent, PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { Actions } from '../../../generated/actions';
import { Observable, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { TitleService } from '../../services/title.service';
import { NotificationService } from '../../../core/services/notification.service';
import { TextInputDialogComponent } from '../../components/text-input-dialog/text-input-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { DiscardChangesDetector } from '../../../core/guards/discard-changes.guard';
import { LocalStorageService } from '../../services/local-storage.service';
import { ApplicationsFilter, defaultApplicationsFilter } from '../../models/applications-filter';
import { NewApplicationDialogComponent } from '../../components/new-application-dialog/new-application-dialog.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { RenameButtonComponent } from '../../components/rename-button/rename-button.component';
import { UnsavedChangesAlertComponent } from '../../components/unsaved-changes-alert/unsaved-changes-alert.component';
import { BadgeComponent } from '../../components/badge/badge.component';
import { TournamentClassManagerComponent } from '../../components/tournament-class-manager/tournament-class-manager.component';
import { TranslateDirective } from '@ngx-translate/core';
import { InvitationLinkTileComponent } from '../../components/invitation-link-tile/invitation-link-tile.component';
import { AlertComponent } from '../../components/alert/alert.component';
import { ManageApplicationsFilterComponent } from '../../components/manage-applications-filter/manage-applications-filter.component';
import { RbacWidgetComponent } from '../../components/rbac-widget/rbac-widget.component';
import { DeleteWidgetComponent } from '../../components/delete-widget/delete-widget.component';
import { ManageApplicationsComponent } from '../../components/manage-applications/manage-applications.component';
import { TournamentPlannerDto } from '../../../api/models/tournament-planner-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { CreateApplicationEndpointRequest } from '../../../api/models/create-application-endpoint-request';
import { UpdateTournamentPlannerEndpointRequest } from '../../../api/models/update-tournament-planner-endpoint-request';
import { getTournamentPlanner } from '../../../api/fn/tournament-planners/get-tournament-planner';
import { createApplication } from '../../../api/fn/applications/create-application';
import { updateTournamentPlanner } from '../../../api/fn/tournament-planners/update-tournament-planner';
import { deleteTournamentPlanner } from '../../../api/fn/tournament-planners/delete-tournament-planner';
import { LabelsManagerComponent } from '../../components/labels-manager/labels-manager.component';
import { ExportApplicationsDialogComponent } from '../../components/export-applications-dialog/export-applications-dialog.component';

export type UpdateTournamentPlannerFunc = (modifyFunc: (tournamentPlanner: TournamentPlannerDto) => boolean) => void;

@Component({
  templateUrl: './view-planning-realm.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    IsActionAllowedDirective,
    ActionButtonComponent,
    TooltipIconComponent,
    RenameButtonComponent,
    UnsavedChangesAlertComponent,
    BadgeComponent,
    TournamentClassManagerComponent,
    TranslateDirective,
    InvitationLinkTileComponent,
    AlertComponent,
    ManageApplicationsFilterComponent,
    RbacWidgetComponent,
    DeleteWidgetComponent,
    ManageApplicationsComponent,
    LabelsManagerComponent
  ]
})
export class ViewPlanningRealmComponent implements OnInit, OnDestroy, DiscardChangesDetector {
  // Note: The color codes are written with '#' such that the IDE detects it as a color code and displays the color preview.
  private static readonly DefaultInvitationLinkColorCodes: string[] = [
    '#ff9900',
    '#ff5500',
    '#bb5500',
    '#55bb00',
    '#ff0055',
    '#bb0055',
    '#5500ff',
    '#9900bb',
    '#00bb99',
    '#0099ff',
    '#0099bb',
    '#0055bb'
  ];

  // Note: The color codes are written with '#' such that the IDE detects it as a color code and displays the color preview.
  public static readonly DefaultLabelColorCodes: string[] = [
    '#FFD1D1',
    '#FFFAD1',
    '#E1FFD1',
    '#D1FDFF',
    '#D1E0FF',
    '#DAD1FF',
    '#FFD1FB',
    '#D1D1D1',
    '#FFFFFF'
  ];

  public static readonly ApplicationsManagerPageId = 2;

  protected readonly Actions = Actions;

  @ViewChild('pageFrame')
  protected pageFrame!: PageFrameComponent;

  protected loadingState: LoadingState = { isLoading: true };
  protected updateFunction: UpdateTournamentPlannerFunc;
  protected tournamentPlanner?: TournamentPlannerDto;
  protected applicationsFilter: ApplicationsFilter = defaultApplicationsFilter;
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
      id: 4,
      title: 'Portal.ViewPlanningRealm.Pages.Labels',
      icon: 'bi-tags'
    },
    {
      id: ViewPlanningRealmComponent.ApplicationsManagerPageId,
      title: 'Portal.ViewPlanningRealm.Pages.Applications',
      icon: 'bi-card-checklist',
      authorization: Actions.ApplicationsRead
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
    private readonly turnierplanApi: TurnierplanApi,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly notificationService: NotificationService,
    private readonly titleService: TitleService,
    private readonly modalService: NgbModal,
    private readonly localStorageService: LocalStorageService
  ) {
    this.updateFunction = (modifyFunc: (tournamentPlanner: TournamentPlannerDto) => boolean) => {
      if (this.tournamentPlanner) {
        if (modifyFunc(this.tournamentPlanner)) {
          this._hasUnsavedChanges = true;
        }
      }
    };
  }

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const tournamentPlannerId = params.get('id');
          if (tournamentPlannerId === null) {
            this.loadingState = { isLoading: false };
            return of();
          }
          this.loadingState = { isLoading: true };
          return this.turnierplanApi.invoke(getTournamentPlanner, { id: tournamentPlannerId });
        })
      )
      .subscribe({
        next: (tournamentPlanner) => {
          this.setTournamentPlanner(tournamentPlanner);
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
        this.updateFunction((tournamentPlanner) => {
          tournamentPlanner.tournamentClasses.push({
            id: this.nextId--,
            name: name.trim(),
            numberOfTeams: 0
          });

          return true;
        });
      }
    });
  }

  protected addInvitationLink(): void {
    this.openModalForEnteringName('NewInvitationLink', true).subscribe({
      next: (name) => {
        this.updateFunction((tournamentPlanner) => {
          tournamentPlanner.invitationLinks.push({
            colorCode: this.getColorCodeForInvitationLink(),
            isActive: true,
            contactEmail: undefined,
            contactPerson: undefined,
            contactTelephone: undefined,
            description: undefined,
            entries: [],
            externalLinks: [],
            id: this.nextId--,
            name: name.trim(),
            numberOfApplications: 0,
            primaryLogo: undefined,
            publicId: '',
            secondaryLogo: undefined,
            title: undefined,
            validUntil: undefined
          });

          return true;
        });
      }
    });
  }

  protected addLabel(): void {
    this.openModalForEnteringName('NewLabel').subscribe({
      next: (name) => {
        this.updateFunction((tournamentPlanner) => {
          tournamentPlanner.labels.push({
            colorCode: this.getColorCodeForLabel(),
            description: '',
            id: this.nextId--,
            name: name.trim()
          });

          return true;
        });
      }
    });
  }

  protected addApplication(): void {
    if (!this.tournamentPlanner || this._hasUnsavedChanges || this.tournamentPlanner.tournamentClasses.length === 0) {
      return;
    }

    const tournamentPlannerId = this.tournamentPlanner.id;

    const ref = this.modalService.open(NewApplicationDialogComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg'
    });

    const component = ref.componentInstance as NewApplicationDialogComponent;
    component.init(this.tournamentPlanner);

    ref.closed
      .pipe(
        tap(() => (this.loadingState = { isLoading: true })),
        switchMap((request: CreateApplicationEndpointRequest) =>
          this.turnierplanApi.invoke(createApplication, { tournamentPlannerId: tournamentPlannerId, body: request }).pipe(map(() => request))
        )
      )
      .subscribe({
        next: (request: CreateApplicationEndpointRequest) => {
          this.loadingState = { isLoading: false };

          // Modify the planning realm stored in the client to account for the newly added teams. By doing this "hack" we can prevent
          // a separate request for querying the entire planning realm when only the "numberOfTeams" property has changed.
          if (this.tournamentPlanner) {
            for (const entry of request.entries) {
              const tournamentClass = this.tournamentPlanner.tournamentClasses?.find((x) => x.id === entry.tournamentClassId);
              if (tournamentClass) {
                tournamentClass.numberOfTeams += entry.numberOfTeams;
              }
            }
          }
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected exportApplications(): void {
    if (!this.tournamentPlanner) {
      return;
    }

    const ref = this.modalService.open(ExportApplicationsDialogComponent, {
      centered: true,
      size: 'md',
      fullscreen: 'md'
    });

    (ref.componentInstance as ExportApplicationsDialogComponent).initialize(this.tournamentPlanner);

    ref.dismissed.subscribe({
      next: (reason?: { isApiError?: boolean; apiError?: unknown }) => {
        if (reason?.isApiError === true) {
          // If reason is specified, this means an error occurred
          this.loadingState = { isLoading: false, error: reason.apiError };
        }
      }
    });
  }

  protected renameTournamentPlanner(name: string): void {
    if (!this.tournamentPlanner) {
      return;
    }

    this.updateFunction((tournamentPlanner) => {
      tournamentPlanner.name = name;
      return true;
    });

    this.titleService.setTitleFrom(this.tournamentPlanner);
  }

  protected saveTournamentPlanner(): void {
    if (!this.tournamentPlanner || this.loadingState.isLoading) {
      return;
    }

    const tournamentPlannerId = this.tournamentPlanner.id;
    this.loadingState = { isLoading: true };

    const request: UpdateTournamentPlannerEndpointRequest = {
      name: this.tournamentPlanner.name,
      tournamentClasses: this.tournamentPlanner.tournamentClasses.map((x) => ({
        id: x.id < 0 ? undefined : x.id,
        name: x.name
      })),
      invitationLinks: this.tournamentPlanner.invitationLinks.map((x) => ({
        id: x.id < 0 ? undefined : x.id,
        name: x.name,
        colorCode: x.colorCode,
        isActive: x.isActive,
        contactEmail: x.contactEmail,
        contactPerson: x.contactPerson,
        contactTelephone: x.contactTelephone,
        description: x.description,
        primaryLogoId: x.primaryLogo?.id,
        secondaryLogoId: x.secondaryLogo?.id,
        title: x.title,
        validUntil: x.validUntil,
        externalLinks: x.externalLinks.map((y) => ({ name: y.name, url: y.url })),
        entries: x.entries.map((y) => ({
          tournamentClassId: y.tournamentClassId,
          maxTeamsPerRegistration: y.maxTeamsPerRegistration,
          allowNewRegistrations: y.allowNewRegistrations
        }))
      })),
      labels: this.tournamentPlanner.labels.map((x) => ({
        id: x.id < 0 ? undefined : x.id,
        name: x.name,
        description: x.description,
        colorCode: x.colorCode
      }))
    };

    this.turnierplanApi
      .invoke(updateTournamentPlanner, { id: tournamentPlannerId, body: request })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getTournamentPlanner, { id: tournamentPlannerId })))
      .subscribe({
        next: (tournamentPlanner) => {
          this.setTournamentPlanner(tournamentPlanner);
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected deleteTournamentPlanner(): void {
    if (!this.tournamentPlanner) {
      return;
    }

    const organizationId = this.tournamentPlanner.organizationId;
    this.loadingState = { isLoading: true, error: undefined };
    this.turnierplanApi.invoke(deleteTournamentPlanner, { id: this.tournamentPlanner.id }).subscribe({
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

  protected onApplicationsFilterChange(filter: ApplicationsFilter): void {
    this.applicationsFilter = filter;

    if (this.tournamentPlanner) {
      this.localStorageService.setTournamentPlannerApplicationsFilter(this.tournamentPlanner.id, filter);
    }
  }

  protected navigateToApplicationsWithFilter(filter: ApplicationsFilter): void {
    this.onApplicationsFilterChange(filter);

    if (this.currentPage !== ViewPlanningRealmComponent.ApplicationsManagerPageId) {
      this.pageFrame.toggleNavigationTab(ViewPlanningRealmComponent.ApplicationsManagerPageId);
    }
  }

  private setTournamentPlanner(tournamentPlanner: TournamentPlannerDto): void {
    this.tournamentPlanner = tournamentPlanner;
    this._hasUnsavedChanges = false;

    this.titleService.setTitleFrom(this.tournamentPlanner);

    this.applicationsFilter = this.localStorageService.getTournamentPlannerApplicationsFilter(tournamentPlanner.id);
  }

  private openModalForEnteringName(key: string, showAlert: boolean = false): Observable<string> {
    const ref = this.modalService.open(TextInputDialogComponent, {
      centered: true,
      size: 'md',
      fullscreen: 'md'
    });

    const component = ref.componentInstance as TextInputDialogComponent;
    component.init(
      `Portal.ViewPlanningRealm.${key}`,
      '',
      false,
      true,
      showAlert ? { type: 'danger', icon: 'exclamation-octagon' } : undefined
    );

    return ref.closed.pipe(map((x) => x as string));
  }

  private getColorCodeForInvitationLink(): string {
    if (!this.tournamentPlanner) {
      return 'aaaaaa';
    }

    return ViewPlanningRealmComponent.getColorCode(
      ViewPlanningRealmComponent.DefaultInvitationLinkColorCodes,
      this.tournamentPlanner.invitationLinks
    );
  }

  private getColorCodeForLabel(): string {
    if (!this.tournamentPlanner) {
      return 'aaaaaa';
    }

    return ViewPlanningRealmComponent.getColorCode(ViewPlanningRealmComponent.DefaultLabelColorCodes, this.tournamentPlanner.labels);
  }

  private static getColorCode(from: string[], currentlyUsed: { colorCode: string }[]): string {
    let availableColorCodes = from.map((x) => x.substring(1)).filter((color) => !currentlyUsed.some((x) => x.colorCode === color));

    if (availableColorCodes.length === 0) {
      availableColorCodes = from.map((x) => x.substring(1));
    }

    return availableColorCodes[Math.floor(Math.random() * availableColorCodes.length)];
  }
}
