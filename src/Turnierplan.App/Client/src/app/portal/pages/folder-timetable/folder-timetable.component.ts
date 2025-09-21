import { formatDate, NgStyle } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, combineLatestWith, delayWhen, interval, of, Subject, switchMap, takeUntil, tap } from 'rxjs';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { LocalStorageService } from '../../services/local-storage.service';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { AlertComponent } from '../../components/alert/alert.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { FormsModule } from '@angular/forms';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';

type TimetableView = {
  hourMarks: number[];
  columnsPerHour: number;
  percentPerColumn: number;
  days: TimetableViewDay[];
};

type TimetableViewDay = {
  date: Date;
  rows: TimetableViewDayRow[];
};

type TimetableViewDayRow = {
  entries: TimetableViewDayRowEntry[];
};

type TimetableViewDayRowEntry = {
  columnsSpacing: number;
  columnsOffset: number;
  columnsCount: number;
  pastMidnight: boolean;
  tournament: FolderTimetableTournamentEntry;
};

@Component({
  templateUrl: './folder-timetable.component.html',
  styleUrl: './folder-timetable.component.scss',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    ActionButtonComponent,
    AlertComponent,
    TranslateDirective,
    NgStyle,
    NgbTooltip,
    TooltipIconComponent,
    FormsModule,
    TranslatePipe,
    TranslateDatePipe
  ]
})
export class FolderTimetableComponent implements OnInit, OnDestroy {
  private static readonly columnWidthMinutes = 1;

  protected loadingState: LoadingState = { isLoading: true };
  protected showRefreshButton = false;
  protected folderName: string = '';
  protected openInNewTab: boolean = true;

  protected view?: TimetableView;
  protected organizationId?: string;
  protected missingTournaments: FolderTimetableTournamentEntry[] = [];

  private readonly refresh$ = new BehaviorSubject<boolean>(false);
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly titleService: TitleService,
    private readonly localStorageService: LocalStorageService
  ) {}

  public ngOnInit(): void {
    this.openInNewTab = this.localStorageService.isOpenTournamentInNewTabEnabled();

    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        combineLatestWith(this.refresh$),
        tap(() => (this.loadingState = { isLoading: true })),
        delayWhen(([, isReload]) => (isReload ? interval(500) : of(undefined))),
        switchMap(([params]) => {
          const folderId = params.get('id') ?? undefined;

          if (!folderId) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }

          this.loadingState = { isLoading: true };

          return this.turnierplanApi.invoke(getFolderTimetable, { id: folderId });
        })
      )
      .subscribe({
        next: (timetable) => {
          if (timetable) {
            this.organizationId = timetable.organizationId;
            this.titleService.setCompoundTitle(timetable.folderName);
            this.generateTimetableView(timetable);

            this.loadingState = { isLoading: false };
            this.showRefreshButton = false;
          }
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();

    this.refresh$.complete();
  }

  protected reloadTimetable(): void {
    if (!this.showRefreshButton) {
      return;
    }

    this.refresh$.next(true);
  }

  protected tournamentLinkClicked(): void {
    setTimeout(() => (this.showRefreshButton = true), 100);
  }

  protected setOpenInNewTab(value: boolean): void {
    this.openInNewTab = value;
    this.localStorageService.setOpenTournamentInNewTab(value);
  }

  private generateTimetableView(timetable: FolderTimetableDto): void {
    this.folderName = timetable.folderName;
    this.missingTournaments = [];

    const formatLocale = 'de';
    const columnsPerHour = 60 / FolderTimetableComponent.columnWidthMinutes;

    const allDates = new Set<string>();

    const mappedTournaments: {
      date: string;
      columnsOffset: number;
      columnsCount: number;
      entry: FolderTimetableTournamentEntry;
      pastMidnight: boolean;
    }[] = [];

    for (const tournament of timetable.tournaments) {
      if (!tournament.startDate || !tournament.endDate) {
        this.missingTournaments.push(tournament);
        continue;
      }

      const startDateConverted = new Date(tournament.startDate);
      const endDateConverted = new Date(tournament.endDate);

      const startDate = formatDate(startDateConverted, 'shortDate', formatLocale);
      const endDate = formatDate(endDateConverted, 'shortDate', formatLocale);

      const minutesOffsetStart = startDateConverted.getHours() * 60 + startDateConverted.getMinutes();
      const minutesOffsetEnd = startDate === endDate ? endDateConverted.getHours() * 60 + endDateConverted.getMinutes() : 24 * 60;

      const columnsOffset = Math.floor(minutesOffsetStart / FolderTimetableComponent.columnWidthMinutes);

      allDates.add(startDate);

      mappedTournaments.push({
        date: startDate,
        columnsOffset: columnsOffset,
        columnsCount: Math.max(Math.ceil(minutesOffsetEnd / FolderTimetableComponent.columnWidthMinutes) - columnsOffset, 1),
        entry: tournament,
        pastMidnight: startDate !== endDate
      });
    }

    const startColumnOffset = Math.min(...mappedTournaments.map((x) => Math.floor(x.columnsOffset / columnsPerHour) * columnsPerHour));
    const maxColumnEnd = Math.max(...mappedTournaments.map((x) => x.columnsOffset + x.columnsCount));

    const startHour = Math.floor(startColumnOffset / columnsPerHour);
    const endHour = Math.ceil(maxColumnEnd / columnsPerHour);

    this.view = {
      hourMarks: [],
      columnsPerHour: columnsPerHour,
      percentPerColumn: 100 / (endHour - startHour) / columnsPerHour,
      days: []
    };

    for (let i = startHour; i <= endHour; i++) {
      this.view.hourMarks.push(i);
    }

    for (const date of allDates) {
      const tournaments = mappedTournaments.filter((x) => x.date === date);

      tournaments.sort((a, b) => {
        const d = a.columnsOffset - b.columnsOffset;
        return d !== 0 ? d : a.columnsCount - b.columnsCount;
      });

      const dateK = new Date(tournaments[0].entry.startDate!);
      dateK.setUTCHours(0, 0, 0, 0);

      const dayResult: TimetableViewDay = {
        date: dateK,
        rows: []
      };

      for (const tournament of tournaments) {
        const relativeTournamentColumnOffset = tournament.columnsOffset - startColumnOffset;

        let row: TimetableViewDayRow | undefined = undefined;

        // Try to find a row that can accept the tournament
        for (const existingRow of dayResult.rows) {
          const canAcceptTournament = !existingRow.entries.some((existingEntry) => {
            // Return true if the existing entry would overlap with the new entry
            return (
              existingEntry.columnsOffset <= relativeTournamentColumnOffset + tournament.columnsCount &&
              relativeTournamentColumnOffset <= existingEntry.columnsOffset + existingEntry.columnsCount
            );
          });

          if (canAcceptTournament) {
            row = existingRow;
            break;
          }
        }

        // If no row is found, add a new row
        if (row === undefined) {
          row = {
            entries: []
          };
          dayResult.rows.push(row);
        }

        // Add the tournament
        row.entries.push({
          columnsSpacing: 0,
          columnsOffset: relativeTournamentColumnOffset,
          columnsCount: tournament.columnsCount,
          pastMidnight: tournament.pastMidnight,
          tournament: tournament.entry
        });
      }

      this.view.days.push(dayResult);
    }

    this.view.days.sort((a, b) => a.date.getTime() - b.date.getTime());

    for (const day of this.view.days) {
      for (const row of day.rows) {
        let previousEnd = 0;

        for (const entry of row.entries) {
          entry.columnsSpacing = entry.columnsOffset - previousEnd;
          previousEnd = entry.columnsOffset + entry.columnsCount;
        }
      }
    }
  }
}
