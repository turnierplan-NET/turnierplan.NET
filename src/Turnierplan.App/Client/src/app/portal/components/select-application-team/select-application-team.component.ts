import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { FormsModule } from '@angular/forms';
import { LocalStorageService } from '../../services/local-storage.service';
import { catchError, of, Subject, switchMap, tap } from 'rxjs';
import { ManageApplicationsFilterComponent } from '../manage-applications-filter/manage-applications-filter.component';
import { ApplicationsFilter, applicationsFilterToQueryParameters, defaultApplicationsFilter } from '../../models/applications-filter';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { PaginationComponent } from '../pagination/pagination.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { ApplicationTeamDto } from '../../../api/models/application-team-dto';
import { PlanningRealmHeaderDto } from '../../../api/models/planning-realm-header-dto';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { PaginationResultDtoOfApplicationDto } from '../../../api/models/pagination-result-dto-of-application-dto';
import { PublicId } from '../../../api/models/public-id';
import { getPlanningRealms } from '../../../api/fn/planning-realms/get-planning-realms';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getPlanningRealm } from '../../../api/fn/planning-realms/get-planning-realm';
import { getApplications } from '../../../api/fn/applications/get-applications';

export type SelectApplicationTeamResult = {
  name: string;
  planningRealmId: string;
  planningRealmName: string;
  tournamentClassName: string;
  applicationTeamId: number;
}[];

@Component({
  selector: 'tp-select-application-team',
  imports: [
    TranslateDirective,
    SmallSpinnerComponent,
    FormsModule,
    ManageApplicationsFilterComponent,
    TooltipIconComponent,
    PaginationComponent,
    TranslateDatePipe
  ],
  templateUrl: './select-application-team.component.html',
  styleUrl: './select-application-team.component.scss'
})
export class SelectApplicationTeamComponent implements OnInit, OnDestroy {
  protected isLoadingPlanningRealms = true;
  protected planningRealms?: PlanningRealmHeaderDto[];
  protected currentPlanningRealmId?: string;
  protected isLoadingPlanningRealmDetail = true;
  protected planningRealmDetail?: PlanningRealmDto;
  protected applicationsFilter: ApplicationsFilter = defaultApplicationsFilter;
  protected isLoadingApplications = true;
  protected applicationsCurrentPage = 0;
  protected applicationsPageSize = 10;
  protected applications?: PaginationResultDtoOfApplicationDto;
  protected currentSelection: SelectApplicationTeamResult = [];

  @Input()
  public organizationId!: PublicId;

  @Input()
  public usedApplicationTeamIds: number[] = [];

  @Output()
  public teamSelected = new EventEmitter<SelectApplicationTeamResult>();

  private readonly planningRealmId$ = new Subject<string>();
  private readonly loadApplications$ = new Subject<void>();

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly localStorageService: LocalStorageService
  ) {}

  public ngOnInit(): void {
    this.isLoadingPlanningRealms = true;

    this.turnierplanApi.invoke(getPlanningRealms, { organizationId: this.organizationId }).subscribe({
      next: (result) => {
        this.planningRealms = result;
        this.isLoadingPlanningRealms = false;

        if (this.planningRealms.length > 0) {
          const currentId = this.localStorageService.getSelectApplicationTeamPlanningRealmId(this.organizationId);

          if (currentId && this.planningRealms.some((x) => x.id === currentId)) {
            this.currentPlanningRealmId = currentId;
            this.onPlanningRealmChange(currentId);
          } else {
            this.currentPlanningRealmId = this.planningRealms[0].id;
            this.onPlanningRealmChange(this.planningRealms[0].id);
          }
        }
      },
      error: () => {
        this.isLoadingPlanningRealms = false;
      }
    });

    this.planningRealmId$
      .pipe(
        tap(() => {
          this.currentSelection = [];
          this.emitSelectedTeams();
        }),
        switchMap((id) => {
          this.isLoadingPlanningRealmDetail = true;

          return this.turnierplanApi.invoke(getPlanningRealm, { id: id });
        }),
        catchError(() => of(undefined))
      )
      .subscribe({
        next: (result) => {
          this.planningRealmDetail = result;
          this.isLoadingPlanningRealmDetail = false;
          this.loadApplications$.next();
        }
      });

    this.loadApplications$
      .pipe(
        switchMap(() => {
          this.isLoadingApplications = true;

          return this.turnierplanApi.invoke(getApplications, {
            planningRealmId: this.planningRealmDetail!.id,
            page: this.applicationsCurrentPage,
            pageSize: this.applicationsPageSize,
            ...applicationsFilterToQueryParameters(this.applicationsFilter)
          });
        }),
        catchError(() => of(undefined))
      )
      .subscribe({
        next: (result) => {
          this.isLoadingApplications = false;
          this.applications = result;
        }
      });
  }

  public ngOnDestroy(): void {
    this.planningRealmId$.complete();
  }

  protected onPlanningRealmChange(id: string): void {
    this.localStorageService.setSelectApplicationTeamPlanningRealmId(this.organizationId, id);
    this.applicationsFilter = this.localStorageService.getPlanningRealmApplicationsFilter(id);
    this.planningRealmId$.next(id);
  }

  protected onApplicationsFilterChange(filter: ApplicationsFilter): void {
    this.applicationsFilter = filter;

    // Always reset to the first page when the filter changes
    this.applicationsCurrentPage = 0;

    // Always clear all selected teams when the filter changes
    this.currentSelection = [];
    this.emitSelectedTeams();

    if (this.planningRealmDetail) {
      this.localStorageService.setPlanningRealmApplicationsFilter(this.planningRealmDetail.id, filter);
    }

    this.loadApplications$.next();
  }

  protected switchPage(page: number): void {
    if (!this.applications || page < 0 || page >= this.applications.totalPages) {
      return;
    }

    this.applicationsCurrentPage = page;
    this.loadApplications$.next();
  }

  protected getTournamentClassName(id: number): string {
    return this.planningRealmDetail?.tournamentClasses.find((x) => x.id === id)?.name ?? '';
  }

  protected filterTeams(teams: ApplicationTeamDto[]): ApplicationTeamDto[] {
    return teams.filter((team) => this.isTeamVisible(team));
  }

  protected setTeamSelected(id: number, selected: boolean, name?: string, tournamentClassName?: string): void {
    this.currentSelection = this.currentSelection.filter((x) => x.applicationTeamId !== id);

    if (selected && name && tournamentClassName && this.planningRealmDetail) {
      this.currentSelection.push({
        name: name,
        planningRealmId: this.planningRealmDetail?.id,
        planningRealmName: this.planningRealmDetail?.name,
        tournamentClassName: tournamentClassName,
        applicationTeamId: id
      });
    }

    this.emitSelectedTeams();
  }

  protected isTeamSelected(id: number): boolean {
    return this.currentSelection.some((x) => x.applicationTeamId === id);
  }

  protected isTeamDisabled(id: number): boolean {
    return this.usedApplicationTeamIds.includes(id);
  }

  private emitSelectedTeams(): void {
    this.teamSelected.emit(this.currentSelection);
  }

  private isTeamVisible(team: ApplicationTeamDto): boolean {
    return this.applicationsFilter.tournamentClass.length === 0 || this.applicationsFilter.tournamentClass.includes(team.tournamentClassId);
  }
}
