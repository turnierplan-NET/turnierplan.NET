import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { ApplicationsFilter, applicationsFilterToQueryParameters } from '../../models/applications-filter';
import { BehaviorSubject, combineLatestWith, ReplaySubject, switchMap, tap } from 'rxjs';
import {
  ApplicationsService,
  PlanningRealmDto,
  PaginationResultDtoOfApplicationDto,
  ApplicationTeamDto,
  ApplicationDto
} from '../../../api';
import { TextInputDialogComponent } from '../text-input-dialog/text-input-dialog.component';
import { NgbModal, NgbTooltip, NgbPagination } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { LoadingIndicatorComponent } from '../loading-indicator/loading-indicator.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { CopyToClipboardComponent } from '../copy-to-clipboard/copy-to-clipboard.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { NgClass } from '@angular/common';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';

// TODO: Endpoint + UI for removing the connection between team & application team (here and in the team-list component)
// TODO: Endpoint + UI for renaming an application team (should also rename Team if a link is active)
// TODO: Display linked teams in applications page incl. link to navigate to the corresponding tournament

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
    NgbPagination,
    TranslatePipe,
    TranslateDatePipe
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

  protected currentPage = 1;
  protected pageSize = 15;
  protected isLoading = false;
  protected result?: PaginationResultDtoOfApplicationDto;
  protected showTeamsApplicationId?: number;
  protected updatingNotesOfApplicationId?: number;

  private readonly filter$ = new ReplaySubject<ApplicationsFilter>();
  private readonly reload$ = new BehaviorSubject<undefined>(undefined);

  private tournamentClassFilter: number[] = [];

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
            ...applicationsFilterToQueryParameters(filter)
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
