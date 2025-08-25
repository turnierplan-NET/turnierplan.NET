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
import { TextInputDialogComponent } from '../text-input-dialog/text-input-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'tp-manage-applications',
  templateUrl: './manage-applications.component.html',
  styleUrl: './manage-applications.component.scss'
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

  protected currentPage = 1;
  protected pageSize = 15;
  protected isLoading = false;
  protected result?: PaginationResultDtoOfApplicationDto;
  protected showTeamsApplicationId?: number;
  protected updatingNotesOfApplicationId?: number;

  private readonly filter$ = new ReplaySubject<ApplicationsFilter>();
  private readonly reload$ = new BehaviorSubject<undefined>(undefined);

  private tournamentClassFilter: TournamentClassFilterValue[] = [];

  constructor(
    private readonly applicationsService: ApplicationsService,
    private readonly modalService: NgbModal
  ) {
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

          if (this.showTeamsApplicationId !== undefined && !result.items.some((x) => x.id === this.showTeamsApplicationId)) {
            this.showTeamsApplicationId = undefined;
          }
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

  public reload(): void {
    this.reload$.next(undefined);
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
      invitationLink: []
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
          this.applicationsService
            .setApplicationNotes({
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
}
