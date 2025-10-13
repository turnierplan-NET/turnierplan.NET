import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { ApplicationsFilter, applicationsFilterToQueryParameters } from '../../models/applications-filter';
import { BehaviorSubject, combineLatestWith, ReplaySubject, switchMap, tap } from 'rxjs';
import { TextInputDialogComponent } from '../text-input-dialog/text-input-dialog.component';
import { NgbModal, NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { LoadingIndicatorComponent } from '../loading-indicator/loading-indicator.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { CopyToClipboardComponent } from '../copy-to-clipboard/copy-to-clipboard.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { AsyncPipe, NgClass, NgStyle } from '@angular/common';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { PaginationComponent } from '../pagination/pagination.component';
import { ViewTournamentComponent } from '../../pages/view-tournament/view-tournament.component';
import { LocalStorageService } from '../../services/local-storage.service';
import { Router } from '@angular/router';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { PaginationResultDtoOfApplicationDto } from '../../../api/models/pagination-result-dto-of-application-dto';
import { getApplications } from '../../../api/fn/applications/get-applications';
import { InvitationLinkDto } from '../../../api/models/invitation-link-dto';
import { ApplicationTeamDto } from '../../../api/models/application-team-dto';
import { ApplicationDto } from '../../../api/models/application-dto';
import { setApplicationNotes } from '../../../api/fn/applications/set-application-notes';
import { PublicId } from '../../../api/models/public-id';
import { setApplicationTeamName } from '../../../api/fn/application-teams/set-application-team-name';
import { LabelDto } from '../../../api/models/label-dto';
import { LabelComponent } from '../label/label.component';
import { LabelsSelectComponent } from '../labels-select/labels-select.component';
import { setApplicationTeamLabels } from '../../../api/fn/application-teams/set-application-team-labels';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { ApplicationChangeLogComponent } from '../application-change-log/application-change-log.component';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { deleteApplicationTeam } from '../../../api/fn/application-teams/delete-application-team';
import { ManageApplicationsAddTeamComponent } from '../manage-applications-add-team/manage-applications-add-team.component';
import { CreateApplicationTeamEndpointRequest } from '../../../api/models/create-application-team-endpoint-request';
import { createApplicationTeam } from '../../../api/fn/application-teams/create-application-team';

@Component({
  selector: 'tp-manage-applications',
  templateUrl: './manage-applications.component.html',
  styleUrl: './manage-applications.component.scss',
  imports: [
    LoadingIndicatorComponent,
    TranslateDirective,
    TooltipIconComponent,
    ActionButtonComponent,
    CopyToClipboardComponent,
    SmallSpinnerComponent,
    NgClass,
    NgbTooltip,
    TranslatePipe,
    TranslateDatePipe,
    PaginationComponent,
    RenameButtonComponent,
    NgStyle,
    LabelComponent,
    AsyncPipe,
    DeleteButtonComponent
  ]
})
export class ManageApplicationsComponent implements OnDestroy {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Input()
  public set filter(value: ApplicationsFilter) {
    this.filter$.next(value);
  }

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();

  protected readonly Actions = Actions;

  protected currentPage = 0;
  protected pageSize = 15;
  protected isLoading = false;
  protected result?: PaginationResultDtoOfApplicationDto;
  protected combinedEmailAddresses?: string = undefined;
  protected expandedApplications: { [key: number]: boolean } = {};
  protected allApplicationsExpanded: boolean = false;
  protected updatingNotesOfApplicationId?: number;
  protected updatingLabelsOfApplicationTeamId?: number;

  private readonly filter$ = new ReplaySubject<ApplicationsFilter>();
  private readonly reload$ = new BehaviorSubject<undefined>(undefined);

  private tournamentClassFilter: number[] = [];

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly turnierplanApi: TurnierplanApi,
    private readonly modalService: NgbModal,
    private readonly localStorageService: LocalStorageService,
    private readonly router: Router
  ) {
    this.filter$
      .pipe(
        tap((filter) => {
          // Always reset to the first page when the filter changes
          this.currentPage = 0;

          this.tournamentClassFilter = filter.tournamentClass;
        }),
        combineLatestWith(this.reload$),
        tap(() => (this.isLoading = true)),
        switchMap(([filter, _]) => {
          return this.turnierplanApi.invoke(getApplications, {
            planningRealmId: this.planningRealm.id,
            page: this.currentPage,
            pageSize: this.pageSize,
            ...applicationsFilterToQueryParameters(filter)
          });
        })
      )
      .subscribe({
        next: (result) => {
          this.result = result;
          this.isLoading = false;

          if (result.items.length === 0) {
            this.combinedEmailAddresses = undefined;
          } else {
            this.combinedEmailAddresses = result.items
              .map((application) => application.contactEmail)
              .filter((mail) => !!mail)
              .join(' ');
          }

          if (result.items.length === 1) {
            // if the page contains 1 item, always expand it immediately
            this.expandedApplications = {};
            this.expandedApplications[result.items[0].id] = true;
          } else if (result.items.length > 0) {
            // if the page contains more than 1 item, expand all items that were previously expanded and are still visible
            const currentlyExpandedIds = Object.keys(this.expandedApplications)
              .map((id) => +id)
              .filter((id) => this.expandedApplications[id]);

            this.expandedApplications = {};

            for (const id of currentlyExpandedIds) {
              if (result.items.some((x) => x.id === id)) {
                this.expandedApplications[id] = true;
              }
            }
          }

          this.determineAllApplicationsExpanded();
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  public ngOnDestroy(): void {
    this.filter$.complete();
    this.reload$.complete();
  }

  protected switchPage(page: number): void {
    if (!this.result || page < 0 || page >= this.result.totalPages) {
      return;
    }

    this.currentPage = page;
    this.reload$.next(undefined);
  }

  protected getTournamentClassName(id: number): string {
    return this.planningRealm.tournamentClasses.find((x) => x.id === id)?.name ?? '?';
  }

  protected getInvitationLink(id: number): InvitationLinkDto {
    return this.planningRealm.invitationLinks.find((x) => x.id === id)!;
  }

  protected getLabel(id: number): LabelDto {
    return this.planningRealm.labels.find((x) => x.id === id)!;
  }

  protected isTeamVisible(team: ApplicationTeamDto): boolean {
    return this.tournamentClassFilter.length === 0 || this.tournamentClassFilter.includes(team.tournamentClassId);
  }

  protected getNumberOfVisibleTeams(application: ApplicationDto): number {
    return application.teams.filter((team) => this.isTeamVisible(team)).length;
  }

  protected getNumberOfHiddenTeams(application: ApplicationDto): number {
    return application.teams.filter((team) => !this.isTeamVisible(team)).length;
  }

  protected setFilterToApplicationTag(applicationTag: number): void {
    this.filterRequested.emit({
      searchTerm: `${applicationTag}`,
      tournamentClass: [],
      invitationLink: [],
      label: []
    });
  }

  protected editApplicationNotes(application: ApplicationDto): void {
    if (this.updatingNotesOfApplicationId !== undefined) {
      return;
    }

    const ref = this.modalService.open(TextInputDialogComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg',
      backdrop: 'static'
    });

    const component = ref.componentInstance as TextInputDialogComponent;
    component.init('Portal.ViewPlanningRealm.Applications.EditNotes', application.notes, true, false);

    ref.closed
      .pipe(
        tap(() => (this.updatingNotesOfApplicationId = application.id)),
        switchMap((notes) =>
          this.turnierplanApi
            .invoke(setApplicationNotes, {
              planningRealmId: this.planningRealm.id,
              applicationId: application.id,
              body: {
                notes: notes
              }
            })
            .pipe(map(() => notes))
        )
      )
      .subscribe({
        next: (notes: string) => {
          application.notes = notes;
          this.updatingNotesOfApplicationId = undefined;
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected showApplicationChangeLog(application: ApplicationDto): void {
    const ref = this.modalService.open(ApplicationChangeLogComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg',
      scrollable: true
    });

    const component = ref.componentInstance as ApplicationChangeLogComponent;
    component.init(this.planningRealm.id, application);

    component.error$.subscribe({
      next: (value) => {
        ref.close();
        this.errorOccured.emit(value);
      }
    });
  }

  protected navigateToTournament(tournamentId: PublicId): void {
    this.localStorageService.setNavigationTab(tournamentId, ViewTournamentComponent.TeamListPageId);
    void this.router.navigate(['/portal/tournament/', tournamentId]);
  }

  protected addTeam(applicationId: number): void {
    const ref = this.modalService.open(ManageApplicationsAddTeamComponent, {
      centered: true,
      size: 'md',
      fullscreen: 'md'
    });

    const component = ref.componentInstance as ManageApplicationsAddTeamComponent;
    component.init(this.planningRealm);

    ref.closed
      .pipe(
        tap(() => (this.isLoading = true)),
        switchMap((request: CreateApplicationTeamEndpointRequest) =>
          this.turnierplanApi.invoke(createApplicationTeam, {
            planningRealmId: this.planningRealm.id,
            applicationId: applicationId,
            body: request
          })
        )
      )
      .subscribe({
        next: () => {
          this.reload$.next(undefined); // reload will eventually set isLoading to false
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected renameTeam(applicationId: number, applicationTeam: ApplicationTeamDto, name: string): void {
    this.turnierplanApi
      .invoke(setApplicationTeamName, {
        planningRealmId: this.planningRealm.id,
        applicationId: applicationId,
        applicationTeamId: applicationTeam.id,
        body: {
          name: name
        }
      })
      .subscribe({
        next: () => {
          applicationTeam.name = name;
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected editTeamLabels(applicationId: number, applicationTeam: ApplicationTeamDto): void {
    const ref = this.modalService.open(LabelsSelectComponent, {
      centered: true,
      size: 'md',
      fullscreen: 'md',
      backdrop: 'static'
    });

    const component = ref.componentInstance as LabelsSelectComponent;
    component.init(this.planningRealm.labels, applicationTeam.labelIds);

    ref.closed
      .pipe(
        tap(() => (this.updatingLabelsOfApplicationTeamId = applicationTeam.id)),
        switchMap((labelIds) =>
          this.turnierplanApi
            .invoke(setApplicationTeamLabels, {
              planningRealmId: this.planningRealm.id,
              applicationId: applicationId,
              applicationTeamId: applicationTeam.id,
              body: { labelIds: labelIds }
            })
            .pipe(map(() => labelIds))
        )
      )
      .subscribe({
        next: (labelIds: number[]) => {
          applicationTeam.labelIds = labelIds;
          this.updatingLabelsOfApplicationTeamId = undefined;
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected deleteTeam(applicationId: number, applicationTeamId: number): void {
    this.isLoading = true;

    this.turnierplanApi
      .invoke(deleteApplicationTeam, {
        planningRealmId: this.planningRealm.id,
        applicationId: applicationId,
        applicationTeamId: applicationTeamId
      })
      .subscribe({
        next: () => {
          this.reload$.next(undefined); // reload will eventually set isLoading to false
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected setAllApplicationsExpanded(expanded: boolean): void {
    this.expandedApplications = {};

    if (this.result) {
      for (const application of this.result.items) {
        this.expandedApplications[application.id] = expanded;
      }
    }

    this.allApplicationsExpanded = expanded;
  }

  protected setApplicationExpanded(id: number, expanded: boolean): void {
    this.expandedApplications[id] = expanded;

    this.determineAllApplicationsExpanded();
  }

  private determineAllApplicationsExpanded(): void {
    this.allApplicationsExpanded = !!this.result && !this.result.items.some((x) => !this.expandedApplications[x.id]);
  }
}
