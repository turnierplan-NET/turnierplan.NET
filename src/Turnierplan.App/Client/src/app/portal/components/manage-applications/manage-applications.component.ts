import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { ApplicationsFilter, TournamentClassFilterValue } from '../../models/applications-filter';
import { BehaviorSubject, combineLatestWith, ReplaySubject, switchMap, tap } from 'rxjs';
import {
  ApplicationsService,
  PlanningRealmDto,
  PaginationResultDtoOfApplicationDto,
  ApplicationTeamDto,
  ApplicationDto
} from '../../../api';

@Component({
  standalone: false,
  selector: 'tp-manage-applications',
  templateUrl: './manage-applications.component.html'
})
export class ManageApplicationsComponent implements OnDestroy {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected currentPage = 1;
  protected pageSize = 15;
  protected isLoading = false;
  protected result?: PaginationResultDtoOfApplicationDto;
  protected showTeamsApplicationId?: number;

  private readonly filter$ = new ReplaySubject<ApplicationsFilter>();
  private readonly reload$ = new BehaviorSubject<undefined>(undefined);

  private tournamentClassFilter: TournamentClassFilterValue[] = [];

  constructor(private readonly applicationsService: ApplicationsService) {
    this.filter$
      .pipe(
        tap((filter) => {
          // Always reset to the first page when the filter changes
          this.currentPage = 1;

          this.tournamentClassFilter = filter.tournamentClass;
        }),
        combineLatestWith(this.reload$),
        tap(() => (this.isLoading = true)),
        switchMap(([filter, _]) => {
          return this.applicationsService.getApplications({
            planningRealmId: this.planningRealm.id,
            page: this.currentPage - 1,
            pageSize: this.pageSize,
            searchTerm: filter.searchTerm.trim() === '' ? undefined : filter.searchTerm,
            tournamentClass: filter.tournamentClass.map((x) => `${x}`),
            invitationLink: filter.invitationLink.map((x) => `${x}`)
          });
        })
      )
      .subscribe({
        next: (result) => {
          this.result = result;
          this.isLoading = false;
          this.showTeamsApplicationId = undefined;
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  @Input()
  public set filter(value: ApplicationsFilter) {
    this.filter$.next(value);
  }

  public ngOnDestroy(): void {
    this.filter$.complete();
    this.reload$.complete();
  }

  protected switchPage(page: number): void {
    if (!this.result || page <= 0 || page > this.result.totalPages) {
      return;
    }

    this.currentPage = page;
    this.reload$.next(undefined);
  }

  protected getTournamentClassName(id: number): string {
    return this.planningRealm.tournamentClasses.find((x) => x.id === id)?.name ?? '?';
  }

  protected isTeamVisible(team: ApplicationTeamDto): boolean {
    return this.tournamentClassFilter.length === 0 || this.tournamentClassFilter.some((x) => x === team.tournamentClassId);
  }

  protected getNumberOfHiddenTeams(application: ApplicationDto): number {
    return application.teams.filter((team) => !this.isTeamVisible(team)).length;
  }
}
