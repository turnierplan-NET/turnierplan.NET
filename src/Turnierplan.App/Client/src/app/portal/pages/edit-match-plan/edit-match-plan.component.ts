import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService, TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { DndDropEvent, DndDropzoneDirective, DndDraggableDirective, DndHandleDirective, DndPlaceholderRefDirective } from 'ngx-drag-drop';
import { combineLatestWith, from, of, Subject, switchMap, takeUntil } from 'rxjs';

import {
  GroupDto,
  MatchState,
  MatchType,
  SetTournamentMatchPlanEndpointRequestEntry,
  TeamDto,
  TeamSelectorDto,
  TournamentDto,
  TournamentsService
} from '../../../api';
import { DiscardChangesDetector } from '../../../core/guards/discard-changes.guard';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { LocalStorageService } from '../../services/local-storage.service';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TournamentEditWarningComponent } from '../../components/tournament-edit-warning/tournament-edit-warning.component';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { FormsModule } from '@angular/forms';
import { NgClass, DatePipe } from '@angular/common';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { ViewTournamentComponent } from '../view-tournament/view-tournament.component';

type ExtendedMatchEntry = SetTournamentMatchPlanEndpointRequestEntry & {
  formattedType: string;
  isGroupMatch: boolean;
  possibleTeamSelectors: string[];
};

@Component({
  templateUrl: './edit-match-plan.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslateDirective,
    TournamentEditWarningComponent,
    DndDropzoneDirective,
    DndDraggableDirective,
    DndHandleDirective,
    ActionButtonComponent,
    FormsModule,
    DndPlaceholderRefDirective,
    NgClass,
    DatePipe,
    TranslatePipe,
    TranslateDatePipe
  ]
})
export class EditMatchPlanComponent implements OnInit, OnDestroy, DiscardChangesDetector {
  // Loading & Dirty state
  protected loadingState: LoadingState = { isLoading: true };
  protected isDirty = false;
  protected tournamentName: string = '';
  protected showKickoffWithSeconds: boolean = false;
  protected groups: GroupDto[] = [];
  protected teams: TeamDto[] = [];

  // User feedback & input
  protected displayEditWarning = false;
  protected editWarningAccepted = false;
  protected currentState: ExtendedMatchEntry[] = [];
  protected currentlyMovingMatchId?: number;
  protected currentlyEditingMatchId?: number;

  // Localized team selectors
  protected localizedTeamSelectors: { [key: string]: string } = {};

  // Private variables
  private readonly destroyed$ = new Subject<void>();
  private tournamentId?: string;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly tournamentService: TournamentsService,
    private readonly titleService: TitleService,
    private readonly notificationService: NotificationService,
    private readonly localStorageService: LocalStorageService,
    private readonly translateService: TranslateService
  ) {}

  protected get canSave(): boolean {
    return !this.displayEditWarning || this.editWarningAccepted;
  }

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          this.tournamentId = params.get('id') ?? undefined;

          if (!this.tournamentId) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }

          this.loadingState = { isLoading: true };

          return this.tournamentService.getTournament({ id: this.tournamentId }).pipe(
            combineLatestWith(
              this.tournamentService.getTournamentTeamSelectors({
                id: this.tournamentId,
                languageCode: this.translateService.getCurrentLang()
              })
            )
          );
        })
      )
      .subscribe({
        next: (data) => {
          if (data) {
            this.extractTournamentData(data[0], data[1]);
            this.loadingState = { isLoading: false };
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
  }

  public hasUnsavedChanges(): boolean {
    return this.isDirty;
  }

  protected saveButtonClicked(): void {
    if (!this.tournamentId || !this.canSave) {
      return;
    }

    const navigateBack = (): Promise<boolean> => {
      // Update local storage so that the user is redirected to the match plan page of the tournament
      if (this.tournamentId) {
        this.localStorageService.setNavigationTab(this.tournamentId, ViewTournamentComponent.matchPlanPageId);
      }

      return this.router.navigate(['..'], { relativeTo: this.route });
    };

    if (!this.isDirty) {
      void navigateBack();
      return;
    }

    this.loadingState = { isLoading: true };

    this.tournamentService
      .setTournamentMatchPlan({
        id: this.tournamentId,
        body: { matches: this.currentState }
      })
      .pipe(
        switchMap(() => {
          this.isDirty = false;
          this.notificationService.showNotification(
            'success',
            'Portal.EditMatchPlan.SuccessToast.Title',
            'Portal.EditMatchPlan.SuccessToast.Message'
          );

          return from(navigateBack());
        })
      )
      .subscribe({
        next: () => {
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected moveMatch(event: DndDropEvent): void {
    const sourceIndex = this.currentState.findIndex((x) => x.id === this.currentlyMovingMatchId);

    if (sourceIndex === -1 || event.index === undefined) {
      return;
    }

    // Calculate required information to update the kickoff times later
    const recalculateKickoffTimes = this.currentState.length > 1 && !this.currentState.some((x) => x.kickoff === undefined);
    const firstMatchKickoff = this.currentState[0].kickoff;
    const matchDurations: { [key: number]: number } = {};
    if (recalculateKickoffTimes) {
      for (let i = 0; i < this.currentState.length - 1; i++) {
        const duration = new Date(this.currentState[i + 1].kickoff!).getTime() - new Date(this.currentState[i].kickoff!).getTime();
        matchDurations[this.currentState[i].id] = duration;
        if (i === this.currentState.length - 2) {
          // For the last match, we assume the same duration as the second-to-last match
          matchDurations[this.currentState[i + 1].id] = duration;
        }
      }
    }

    // Get the match we want to move and remove it from the state
    const targetMatch = this.currentState.splice(sourceIndex, 1)[0];

    // Get the index where we want to insert the previously removed match
    const insertAfterIndex = event.index >= sourceIndex ? event.index - 1 : event.index;

    // Insert the match into the state
    this.currentState.splice(insertAfterIndex, 0, targetMatch);

    // Re-calculate the indices & kickoff times of all matches to be sequential and starting from 1
    let nextKickoff = new Date(firstMatchKickoff!);
    for (let i = 0; i < this.currentState.length; i++) {
      this.currentState[i].index = i + 1;
      if (recalculateKickoffTimes) {
        this.currentState[i].kickoff = nextKickoff.toISOString();
        nextKickoff = new Date(nextKickoff.getTime() + matchDurations[this.currentState[i].id]);
      }
    }

    this.isDirty = true;
  }

  protected setMatchCourt(id: number, court: number): void {
    if (court < 0) {
      return;
    }

    const match = this.currentState.find((x) => x.id === id);

    if (match) {
      match.court = court;
      this.isDirty = true;
    }
  }

  protected setMatchKickoff(id: number, value: string): void {
    const match = this.currentState.find((x) => x.id === id);

    if (match) {
      match.kickoff = new Date(value).toISOString();
      this.isDirty = true;
    }
  }

  protected setMatchTeam(id: number, which: 'A' | 'B', teamSelector: string): void {
    const match = this.currentState.find((x) => x.id === id);

    if (match) {
      switch (which) {
        case 'A':
          match.teamSelectorA = teamSelector;
          break;
        case 'B':
          match.teamSelectorB = teamSelector;
          break;
      }

      this.isDirty = true;
    }
  }

  private extractTournamentData(tournament: TournamentDto, teamSelectors: TeamSelectorDto[]): void {
    this.tournamentName = tournament.name;
    this.titleService.setTitleFrom(tournament);
    this.groups = tournament.groups;
    this.teams = tournament.teams;

    this.currentState = [];
    this.showKickoffWithSeconds = false;

    for (const match of tournament.matches) {
      let possibleTeamSelectors: string[];

      if (match.type === MatchType.GroupMatch) {
        const prefix = `G${match.groupId}`;
        possibleTeamSelectors = teamSelectors.map((x) => x.key).filter((x) => x.startsWith(prefix));
      } else {
        possibleTeamSelectors = teamSelectors.map((x) => x.key).filter((x) => x.at(0) !== 'G');
      }

      this.currentState.push({
        id: match.id,
        index: match.index,
        court: match.court,
        kickoff: match.kickoff,
        teamSelectorA: match.teamA.teamSelector.key,
        teamSelectorB: match.teamB.teamSelector.key,
        formattedType: match.formattedType,
        isGroupMatch: match.type === MatchType.GroupMatch,
        possibleTeamSelectors: possibleTeamSelectors
      });

      if (match.kickoff && new Date(match.kickoff).getSeconds() !== 0) {
        this.showKickoffWithSeconds = true;
      }
    }

    this.displayEditWarning = tournament.matches.some((x) => x.state === MatchState.CurrentlyPlaying || x.state === MatchState.Finished);

    this.localizedTeamSelectors = {};

    for (const teamSelector of teamSelectors) {
      this.localizedTeamSelectors[teamSelector.key] = teamSelector.localized;
    }
  }
}
