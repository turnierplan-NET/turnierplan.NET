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
import { TournamentPlannerHeaderDto } from '../../../api/models/tournament-planner-header-dto';
import { TournamentPlannerDto } from '../../../api/models/tournament-planner-dto';
import { PaginationResultDtoOfApplicationDto } from '../../../api/models/pagination-result-dto-of-application-dto';
import { PublicId } from '../../../api/models/public-id';
import { getTournamentPlanners } from '../../../api/fn/tournament-planners/get-tournament-planners';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getTournamentPlanner } from '../../../api/fn/tournament-planners/get-tournament-planner';
import { getApplications } from '../../../api/fn/applications/get-applications';
import { LabelDto } from '../../../api/models/label-dto';
import { LabelComponent } from '../label/label.component';

export type SelectApplicationTeamResult = {
  name: string;
  tournamentPlannerId: string;
  tournamentPlannerName: string;
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
    TranslateDatePipe,
    LabelComponent
  ],
  templateUrl: './select-application-team.component.html',
  styleUrl: './select-application-team.component.scss'
})
export class SelectApplicationTeamComponent implements OnInit, OnDestroy {
  protected isLoadingTournamentPlanners = true;
  protected tournamentPlanners?: TournamentPlannerHeaderDto[];
  protected currentTournamentPlannerId?: string;
  protected isLoadingTournamentPlannerDetail = true;
  protected tournamentPlannerDetail?: TournamentPlannerDto;
  protected applicationsFilter: ApplicationsFilter = defaultApplicationsFilter;
  protected isLoadingApplications = true;
  protected applicationsCurrentPage = 0;
  protected applicationsPageSize = 10;
  protected applications?: PaginationResultDtoOfApplicationDto;
  protected visibleTeamWithExistingLinkIds: number[] = [];
  protected currentSelection: SelectApplicationTeamResult = [];

  @Input()
  public organizationId!: PublicId;

  @Input()
  public usedApplicationTeamIds: number[] = [];

  @Output()
  public teamSelected = new EventEmitter<SelectApplicationTeamResult>();

  private readonly tournamentPlannerId$ = new Subject<string>();
  private readonly loadApplications$ = new Subject<void>();

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly localStorageService: LocalStorageService
  ) {}

  public ngOnInit(): void {
    this.isLoadingTournamentPlanners = true;

    this.turnierplanApi.invoke(getTournamentPlanners, { organizationId: this.organizationId }).subscribe({
      next: (result) => {
        this.tournamentPlanners = result;
        this.isLoadingTournamentPlanners = false;

        if (this.tournamentPlanners.length > 0) {
          const currentId = this.localStorageService.getSelectApplicationTeamTournamentPlannerId(this.organizationId);

          if (currentId && this.tournamentPlanners.some((x) => x.id === currentId)) {
            this.currentTournamentPlannerId = currentId;
            this.onTournamentPlannerChange(currentId);
          } else {
            this.currentTournamentPlannerId = this.tournamentPlanners[0].id;
            this.onTournamentPlannerChange(this.tournamentPlanners[0].id);
          }
        }
      },
      error: () => {
        this.isLoadingTournamentPlanners = false;
      }
    });

    this.tournamentPlannerId$
      .pipe(
        tap(() => {
          this.currentSelection = [];
          this.emitSelectedTeams();
        }),
        switchMap((id) => {
          this.isLoadingTournamentPlannerDetail = true;

          return this.turnierplanApi.invoke(getTournamentPlanner, { id: id });
        }),
        catchError(() => of(undefined))
      )
      .subscribe({
        next: (result) => {
          this.tournamentPlannerDetail = result;
          this.isLoadingTournamentPlannerDetail = false;
          this.loadApplications$.next();
        }
      });

    this.loadApplications$
      .pipe(
        switchMap(() => {
          this.isLoadingApplications = true;

          return this.turnierplanApi.invoke(getApplications, {
            tournamentPlannerId: this.tournamentPlannerDetail!.id,
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

          if (result) {
            this.visibleTeamWithExistingLinkIds = result.items
              .flatMap((x) => x.teams)
              .filter((x) => x.linkedTournament !== undefined)
              .map((x) => x.id);
          }
        }
      });
  }

  public ngOnDestroy(): void {
    this.tournamentPlannerId$.complete();
  }

  protected onTournamentPlannerChange(id: string): void {
    this.localStorageService.setSelectApplicationTeamTournamentPlannerId(this.organizationId, id);
    this.applicationsFilter = this.localStorageService.getTournamentPlannerApplicationsFilter(id);
    this.tournamentPlannerId$.next(id);
  }

  protected onApplicationsFilterChange(filter: ApplicationsFilter): void {
    this.applicationsFilter = filter;

    // Always reset to the first page when the filter changes
    this.applicationsCurrentPage = 0;

    // Always clear all selected teams when the filter changes
    this.currentSelection = [];
    this.emitSelectedTeams();

    if (this.tournamentPlannerDetail) {
      this.localStorageService.setTournamentPlannerApplicationsFilter(this.tournamentPlannerDetail.id, filter);
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
    return this.tournamentPlannerDetail?.tournamentClasses.find((x) => x.id === id)?.name ?? '';
  }

  protected getLabel(id: number): LabelDto {
    return this.tournamentPlannerDetail?.labels.find((x) => x.id === id)!;
  }

  protected filterTeams(teams: ApplicationTeamDto[]): ApplicationTeamDto[] {
    return teams.filter((team) => this.isTeamVisible(team));
  }

  protected setTeamSelected(id: number, selected: boolean, name?: string, tournamentClassName?: string): void {
    this.currentSelection = this.currentSelection.filter((x) => x.applicationTeamId !== id);

    if (selected && name && tournamentClassName && this.tournamentPlannerDetail) {
      this.currentSelection.push({
        name: name,
        tournamentPlannerId: this.tournamentPlannerDetail?.id,
        tournamentPlannerName: this.tournamentPlannerDetail?.name,
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
    return this.usedApplicationTeamIds.includes(id) || this.visibleTeamWithExistingLinkIds.includes(id);
  }

  private emitSelectedTeams(): void {
    this.teamSelected.emit(this.currentSelection);
  }

  private isTeamVisible(team: ApplicationTeamDto): boolean {
    return this.applicationsFilter.tournamentClass.length === 0 || this.applicationsFilter.tournamentClass.includes(team.tournamentClassId);
  }
}
