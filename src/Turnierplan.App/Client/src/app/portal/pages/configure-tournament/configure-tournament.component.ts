import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { combineLatestWith, from, of, Subject, switchMap, takeUntil } from 'rxjs';

import {
  AdditionalPlayoffDto,
  ConfigureTournamentEndpointRequest,
  ConfigureTournamentEndpointRequestGroupEntry,
  ConfigureTournamentEndpointRequestTeamEntry,
  FinalsMatchDefinitionDto,
  FinalsPhaseConfigurationDto,
  MatchPlanConfigurationDto,
  MatchState,
  TournamentDto
} from '../../../api';
import { DiscardChangesDetector } from '../../../core/guards/discard-changes.guard';
import { NotificationService } from '../../../core/services/notification.service';
import { ValidationErrorDialogComponent } from '../../components/validation-error-dialog/validation-error-dialog.component';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { LocalStorageService } from '../../services/local-storage.service';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { TournamentEditWarningComponent } from '../../components/tournament-edit-warning/tournament-edit-warning.component';
import { NgTemplateOutlet, NgClass, DatePipe } from '@angular/common';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { FormsModule } from '@angular/forms';
import { AlertComponent } from '../../components/alert/alert.component';
import { DeleteButtonComponent } from '../../components/delete-button/delete-button.component';
import { DurationPickerComponent } from '../../components/duration-picker/duration-picker.component';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { AbstractTeamSelectorPipe } from '../../pipes/abstract-team-selector.pipe';
import { RenameButtonComponent } from '../../components/rename-button/rename-button.component';
import { ConfigureTournamentAddTeamComponent } from '../../components/configure-tournament-add-team/configure-tournament-add-team.component';
import { ViewTournamentComponent } from '../view-tournament/view-tournament.component';

interface TemporaryGroup {
  id?: number;
  alphabeticalId: string;
  displayName: string;
  teams: TemporaryTeam[];
}

export interface TemporaryTeamLink {
  planningRealmId: string;
  planningRealmName: string;
  tournamentClassName: string;
  applicationTeamId: number;
}

export interface TemporaryTeam {
  id?: number;
  name: string;
  teamLink?: TemporaryTeamLink;
}

enum FirstFinalRound {
  Final = 'Final',
  SemiFinal = 'SemiFinal',
  QuarterFinal = 'QuarterFinal',
  EighthFinal = 'EighthFinal'
}

interface TemporaryAdditionalPlayoff {
  isEnabled: boolean;
  teamSelectorA: string;
  teamSelectorB: string;
}

@Component({
  templateUrl: './configure-tournament.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslateDirective,
    TournamentEditWarningComponent,
    NgTemplateOutlet,
    NgClass,
    ActionButtonComponent,
    FormsModule,
    AlertComponent,
    DeleteButtonComponent,
    DurationPickerComponent,
    TooltipIconComponent,
    DatePipe,
    TranslatePipe,
    AbstractTeamSelectorPipe,
    NgbPopover,
    RenameButtonComponent
  ]
})
export class ConfigureTournamentComponent implements OnInit, OnDestroy, DiscardChangesDetector {
  private static readonly allowedAlphabeticalIds = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.split('');

  // Loading & Dirty state
  protected loadingState: LoadingState = { isLoading: false };
  protected isDirty = false;

  // User feedback & input
  protected firstFinalRounds = FirstFinalRound;
  protected displayEditWarning = false;
  protected editWarningAccepted = false;
  protected attemptedToSaveChanges = false;
  protected availableFinalRounds: FirstFinalRound[] = [];
  protected abstractTeamSelectors: string[] = [];
  protected groupAlphabeticalIdsForTeamSelectors: string[] = [];
  protected teamForMovingToOtherGroup?: { team: TemporaryTeam; currentGroup: TemporaryGroup };
  protected timeStampToday: Date;

  // Current tournament configuration
  protected firstMatchKickoff: Date = new Date();
  protected groups: TemporaryGroup[] = [];
  protected groupPhaseAlternating = false;
  protected groupPhaseCourts: number = 1;
  protected groupPhaseRounds: number = 1;
  protected groupPhasePlayTime: string = '00:10:00';
  protected groupPhasePauseTime: string = '00:01:00';
  protected enableFinalsRound = false;
  protected pauseBetweenGroupAndFinalsPhase: string = '00:05:00';
  protected finalsPhaseCourts: number = 1;
  protected currentFinalRound?: FirstFinalRound;
  protected enableThirdPlacePlayoff = false;
  protected finalsPhasePlayTime: string = '00:10:00';
  protected finalsPhasePauseTime: string = '00:01:00';
  protected enableAdditionalPlayoffs = false;
  protected additionalPlayoffPositions: number[] = [];
  protected additionalPlayoffs: { [position: number]: TemporaryAdditionalPlayoff } = {};

  // Private variables
  private readonly destroyed$ = new Subject<void>();
  private finalsMatchDefinitions?: FinalsMatchDefinitionDto[];
  private originalTournament?: TournamentDto;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly notificationService: NotificationService,
    private readonly titleService: TitleService,
    private readonly localStorageService: LocalStorageService,
    private readonly modalService: NgbModal
  ) {
    const now = new Date();
    this.timeStampToday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  }

  protected get tournamentName(): string {
    return this.originalTournament?.name ?? '';
  }

  protected get totalTeamCount(): number {
    let count = 0;
    this.groups.forEach((x) => (count += x.teams.length));
    return count;
  }

  protected get isGroupPhaseCourtsInvalid(): boolean {
    return this.groupPhaseCourts < 1 || this.groupPhaseCourts > 10;
  }

  protected get isGroupPhaseRoundsInvalid(): boolean {
    return this.groupPhaseRounds < 1 || this.groupPhaseRounds > 4;
  }

  protected get isFinalsPhaseCourtsInvalid(): boolean {
    return this.finalsPhaseCourts < 1 || this.finalsPhaseCourts > 10;
  }

  protected get isAdditionalPlayoffConfigInvalid(): boolean {
    return (
      this.enableFinalsRound &&
      this.currentFinalRound !== undefined &&
      this.enableAdditionalPlayoffs &&
      this.additionalPlayoffPositions.some(
        (position) =>
          this.additionalPlayoffs[position].isEnabled &&
          (this.additionalPlayoffs[position].teamSelectorA === '' || this.additionalPlayoffs[position].teamSelectorB === '')
      )
    );
  }

  protected get hasValidationErrors(): boolean {
    if (
      this.isGroupPhaseCourtsInvalid ||
      this.isGroupPhaseRoundsInvalid ||
      this.isFinalsPhaseCourtsInvalid ||
      this.isAdditionalPlayoffConfigInvalid
    ) {
      return true;
    }

    if (this.attemptedToSaveChanges) {
      return this.groups.length === 0 || this.groups.some((x) => x.teams.length === 0);
    }

    return false;
  }

  protected get canSave(): boolean {
    return (!this.displayEditWarning || this.editWarningAccepted) && !this.hasValidationErrors;
  }

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const tournamentId = params.get('id');
          if (tournamentId === null) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }
          this.loadingState = { isLoading: true };
          return this.tournamentService.getTournament({ id: tournamentId });
        }),
        combineLatestWith(this.metadataService.getAvailableFinalsMatchDefinitions())
      )
      .subscribe({
        next: ([tournament, finalsMatchDefinitions]) => {
          this.extractTournamentData(tournament!);
          this.extractMatchPlanConfiguration(tournament!.matchPlanConfiguration);
          this.finalsMatchDefinitions = finalsMatchDefinitions;
          this.determineAvailableFinalsRounds();
          this.determineAvailableAbstractTeamSelectors();
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
    return this.isDirty;
  }

  protected saveButtonClicked(): void {
    this.attemptedToSaveChanges = true;

    if (!this.canSave || !this.originalTournament) {
      return;
    }

    this.loadingState = { isLoading: true };

    const request = this.createRequest();

    this.tournamentService
      .configureTournament({ id: this.originalTournament.id, body: request })
      .pipe(
        switchMap(() => {
          this.isDirty = false;
          this.notificationService.showNotification(
            'success',
            'Portal.ConfigureTournament.SuccessToast.Title',
            'Portal.ConfigureTournament.SuccessToast.Message'
          );

          // Update local storage so that the user is redirected to the match plan page of the tournament
          if (this.originalTournament) {
            this.localStorageService.setNavigationTab(this.originalTournament.id, ViewTournamentComponent.MatchPlanPageId);
          }

          return from(this.router.navigate(['..'], { relativeTo: this.route }));
        })
      )
      .subscribe({
        next: () => {
          this.loadingState = { isLoading: false };
        },
        error: (error: { error?: string }) => {
          if (error.error) {
            let parsedError: unknown;

            try {
              parsedError = JSON.parse(error.error);
            } catch (e) {
              this.loadingState = { isLoading: false, error: error.error };
              return;
            }

            const validationErrors = (parsedError as { errors?: { [key: string]: string[] } })?.errors;

            if (validationErrors) {
              const ref = this.modalService.open(ValidationErrorDialogComponent, {
                size: 'md',
                fullscreen: 'md',
                centered: true
              });

              (ref.componentInstance as ValidationErrorDialogComponent).errors = validationErrors;

              this.loadingState = { isLoading: false };
            } else {
              this.loadingState = { isLoading: false, error: error.error };
            }
          } else {
            this.loadingState = { isLoading: false, error: error };
          }
        }
      });
  }

  protected setKickoffDatetime(value: string): void {
    this.firstMatchKickoff = new Date(value);
    this.markDirty();
  }

  protected addTeam(group: TemporaryGroup): void {
    if (!this.originalTournament) {
      return;
    }

    const ref = this.modalService.open(ConfigureTournamentAddTeamComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg'
    });

    const usedApplicationTeamIds = this.groups
      .flatMap((x) => x.teams)
      .map((x) => x.teamLink?.applicationTeamId)
      .filter((x) => x !== undefined);

    const component = ref.componentInstance as ConfigureTournamentAddTeamComponent;
    component.init(this.originalTournament.organizationId, usedApplicationTeamIds);

    ref.closed.subscribe({
      next: (teams: TemporaryTeam[]) => {
        group.teams.push(...teams);

        this.determineAvailableFinalsRounds();
        this.determineAvailableAbstractTeamSelectors();
        this.markDirty();
      }
    });
  }

  protected addGroup(): void {
    let nextAlphabeticalId: string | undefined;

    for (const alphabeticalId of ConfigureTournamentComponent.allowedAlphabeticalIds) {
      if (!this.groups.some((x) => x.alphabeticalId === alphabeticalId)) {
        nextAlphabeticalId = alphabeticalId;
        break;
      }
    }

    if (!nextAlphabeticalId) {
      this.notificationService.showNotification(
        'warning',
        'Portal.ConfigureTournament.ConfigurationError',
        'Portal.ConfigureTournament.Sections.Participants.GroupLimitReached'
      );
      return;
    }

    this.groups.push({
      alphabeticalId: nextAlphabeticalId,
      displayName: '',
      id: undefined,
      teams: []
    });

    this.sortGroupsByAlphabeticalId();
    this.determineAvailableFinalsRounds();
    this.determineAvailableAbstractTeamSelectors();
    this.markDirty();
  }

  protected setTeamName(team: TemporaryTeam, name: string): void {
    team.name = name;
    this.markDirty();
  }

  protected setGroupName(group: TemporaryGroup, name: string): void {
    group.displayName = name;
    this.markDirty();
  }

  protected moveTeam(team: TemporaryTeam, from: TemporaryGroup, to: TemporaryGroup): void {
    const index = from.teams.findIndex((x) => x === team);
    from.teams.splice(index, 1);
    to.teams.push(team);

    this.determineAvailableFinalsRounds();
    this.determineAvailableAbstractTeamSelectors();
    this.markDirty();
  }

  protected removeTeam(group: TemporaryGroup, teamIndex: number): void {
    group.teams.splice(teamIndex, 1);

    this.determineAvailableFinalsRounds();
    this.determineAvailableAbstractTeamSelectors();
    this.markDirty();
  }

  protected swapTeams(group: TemporaryGroup, teamIndex: number, newTeamIndex: number): void {
    const swapWith = group.teams[newTeamIndex];
    group.teams[newTeamIndex] = group.teams[teamIndex];
    group.teams[teamIndex] = swapWith;

    this.markDirty();
  }

  protected removeGroup(groupIndex: number): void {
    if (this.groups[groupIndex].teams.length > 0) {
      return;
    }

    this.groups.splice(groupIndex, 1);

    this.determineAvailableFinalsRounds();
    this.determineAvailableAbstractTeamSelectors();
    this.markDirty();
  }

  protected shuffleGroups(): void {
    const teams = this.groups.flatMap((x) => x.teams);
    const teamCount = teams.length;
    const shuffledTeams: TemporaryTeam[] = [];

    for (let i = 0; i < teamCount; i++) {
      const j = Math.floor(Math.random() * teams.length);
      const team = teams.splice(j, 1)[0];
      shuffledTeams.push(team);
    }

    let nextTeamId = 0;
    for (const group of this.groups) {
      for (let i = 0; i < group.teams.length; i++) {
        group.teams[i] = shuffledTeams[nextTeamId++];
      }
    }

    this.markDirty();
  }

  protected determineAvailableAdditionalPlayoffMatches(): void {
    this.additionalPlayoffPositions = [];

    if (this.currentFinalRound === undefined) {
      return;
    }

    if (this.currentFinalRound !== FirstFinalRound.Final && !this.enableThirdPlacePlayoff) {
      return;
    }

    const teamCount = this.totalTeamCount;

    for (let playoffPosition = 3; playoffPosition < teamCount; playoffPosition += 2) {
      if (playoffPosition === 3 && this.currentFinalRound !== FirstFinalRound.Final && this.enableThirdPlacePlayoff) {
        continue;
      }

      this.additionalPlayoffPositions.push(playoffPosition);

      this.additionalPlayoffs[playoffPosition] ??= {
        isEnabled: false,
        teamSelectorA: '',
        teamSelectorB: ''
      };
    }
  }

  protected setAdditionalPlayoffEnabled(position: number, isEnabled: boolean): void {
    this.additionalPlayoffs[position].isEnabled = isEnabled;
    this.markDirty();
  }

  protected setAdditionalPlayoffTeamSelectorA(position: number, teamSelector: string): void {
    this.additionalPlayoffs[position].teamSelectorA = teamSelector;
    this.markDirty();
  }

  protected setAdditionalPlayoffTeamSelectorB(position: number, teamSelector: string): void {
    this.additionalPlayoffs[position].teamSelectorB = teamSelector;
    this.markDirty();
  }

  protected markDirty(): void {
    this.isDirty = true;
  }

  private extractTournamentData(result: TournamentDto): void {
    this.originalTournament = result;
    this.titleService.setTitleFrom(result);
    this.displayEditWarning = result.matches.some((x) => x.state === MatchState.CurrentlyPlaying || x.state === MatchState.Finished);

    this.groups = result.groups.map((group) => {
      return {
        id: group.id,
        alphabeticalId: group.alphabeticalId,
        displayName: group.hasCustomDisplayName ? group.displayName : '',
        teams: group.participants.map((participant): TemporaryTeam => {
          const team = result.teams.find((x) => x.id === participant.teamId);

          if (!team) {
            return {
              id: participant.teamId,
              name: '??'
            };
          }

          if (team.link) {
            return {
              id: participant.teamId,
              name: team.name,
              teamLink: {
                planningRealmId: team.link.planningRealmId,
                planningRealmName: team.link.planningRealmName,
                tournamentClassName: team.link.tournamentClassName,
                applicationTeamId: team.link.applicationTeamId
              }
            };
          }

          return {
            id: participant.teamId,
            name: team.name
          };
        })
      };
    });

    this.sortGroupsByAlphabeticalId();
    this.determineAvailableFinalsRounds();
  }

  private extractMatchPlanConfiguration(config: MatchPlanConfigurationDto): void {
    if (config.firstMatchKickoff) {
      this.firstMatchKickoff = new Date(config.firstMatchKickoff);
    } else {
      // Set seconds to zero to avoid scuffed kickoff times
      const now = new Date();
      this.firstMatchKickoff = new Date(now.getFullYear(), now.getMonth(), now.getDate(), now.getHours(), now.getMinutes(), 0);
    }

    this.groupPhaseAlternating = config.groupPhaseConfig?.useAlternatingOrder === true;
    this.groupPhaseCourts = config.groupPhaseConfig?.numberOfCourts ?? 1;
    this.groupPhaseRounds = config.groupPhaseConfig?.numberOfGroupRounds ?? 1;
    this.groupPhasePlayTime = config.groupPhaseConfig?.schedule?.playTime ?? '00:10:00';
    this.groupPhasePauseTime = config.groupPhaseConfig?.schedule?.pauseTime ?? '00:01:00';

    this.enableFinalsRound = config.finalsPhaseConfig !== undefined;
    this.pauseBetweenGroupAndFinalsPhase = config.pauseBetweenGroupAndFinalsPhase ?? '00:05:00';
    this.finalsPhaseCourts = config.finalsPhaseConfig?.numberOfCourts ?? 1;

    // This mapping is according to FinalsRoundConfig.cs
    switch (config.finalsPhaseConfig?.firstFinalsRound) {
      case 0:
        this.currentFinalRound = FirstFinalRound.Final;
        break;
      case 1:
        this.currentFinalRound = FirstFinalRound.SemiFinal;
        break;
      case 2:
        this.currentFinalRound = FirstFinalRound.QuarterFinal;
        break;
      case 3:
        this.currentFinalRound = FirstFinalRound.EighthFinal;
        break;
    }

    this.enableThirdPlacePlayoff = config.finalsPhaseConfig?.thirdPlacePlayoff === true;
    this.finalsPhasePlayTime = config.finalsPhaseConfig?.schedule?.playTime ?? '00:10:00';
    this.finalsPhasePauseTime = config.finalsPhaseConfig?.schedule?.pauseTime ?? '00:01:00';

    const playoffs = config.finalsPhaseConfig?.additionalPlayoffs ?? [];
    this.enableAdditionalPlayoffs = playoffs.length > 0;
    this.additionalPlayoffs = {};
    this.determineAvailableAdditionalPlayoffMatches();
    for (const position of this.additionalPlayoffPositions) {
      const playoff = playoffs.find((x) => x.playoffPosition === position);
      this.additionalPlayoffs[position] = {
        isEnabled: playoff !== undefined,
        teamSelectorA: playoff?.teamSelectorA ?? '',
        teamSelectorB: playoff?.teamSelectorB ?? ''
      };
    }
  }

  private sortGroupsByAlphabeticalId(): void {
    this.groups.sort((a, b) => a.alphabeticalId.localeCompare(b.alphabeticalId));
  }

  private determineAvailableFinalsRounds(): void {
    this.availableFinalRounds = [];

    if (!this.finalsMatchDefinitions) {
      this.currentFinalRound = undefined;
      this.determineAvailableAdditionalPlayoffMatches();
      return;
    }

    const filteredEntries = this.finalsMatchDefinitions.filter((entry) => {
      if (entry.groupCount !== this.groups.length) {
        return false;
      }

      return this.groups.every((group) => group.teams.length >= entry.requiredTeamsPerGroup);
    });

    for (const entry of filteredEntries) {
      switch (entry.matchCount) {
        case 1:
          this.availableFinalRounds.push(FirstFinalRound.Final);
          break;
        case 2:
          this.availableFinalRounds.push(FirstFinalRound.SemiFinal);
          break;
        case 4:
          this.availableFinalRounds.push(FirstFinalRound.QuarterFinal);
          break;
        case 8:
          this.availableFinalRounds.push(FirstFinalRound.EighthFinal);
          break;
      }
    }

    if (this.availableFinalRounds.length === 0) {
      this.currentFinalRound = undefined;
      this.determineAvailableAdditionalPlayoffMatches();
      return;
    }

    if (!this.currentFinalRound || this.availableFinalRounds.findIndex((x) => x === this.currentFinalRound) === -1) {
      this.currentFinalRound = this.availableFinalRounds[0];
    }

    this.determineAvailableAdditionalPlayoffMatches();
  }

  private determineAvailableAbstractTeamSelectors(): void {
    this.abstractTeamSelectors = [];
    this.groupAlphabeticalIdsForTeamSelectors = this.groups.map((x) => x.alphabeticalId);

    let maxTeamsPerGroup = 0;

    for (let i = 0; i < this.groups.length; i++) {
      const group = this.groups[i];

      for (let j = 0; j < group.teams.length; j++) {
        this.abstractTeamSelectors.push(`${j + 1}.${i}`);
      }

      if (group.teams.length > maxTeamsPerGroup) {
        maxTeamsPerGroup = group.teams.length;
      }
    }

    if (this.groups.length > 1) {
      for (let position = 1; position <= maxTeamsPerGroup; position++) {
        let numberOfSuitableGroups = 0;

        for (const group of this.groups) {
          if (group.teams.length >= position) {
            numberOfSuitableGroups++;
          }
        }

        for (let i = 0; i < numberOfSuitableGroups; i++) {
          this.abstractTeamSelectors.push(`${i}B${position}`);
        }
      }
    }
  }

  private createRequest(): ConfigureTournamentEndpointRequest {
    const convertFirstFinalsRound = (): number => {
      // This mapping is according to FinalsRoundConfig.cs
      switch (this.currentFinalRound) {
        case FirstFinalRound.Final:
          return 0;
        case FirstFinalRound.SemiFinal:
          return 1;
        case FirstFinalRound.QuarterFinal:
          return 2;
        case FirstFinalRound.EighthFinal:
          return 3;
      }
      return -1;
    };

    const finalsPhaseConfig: FinalsPhaseConfigurationDto | undefined =
      this.enableFinalsRound && this.currentFinalRound !== undefined
        ? {
            schedule: {
              playTime: this.finalsPhasePlayTime,
              pauseTime: this.finalsPhasePauseTime
            },
            numberOfCourts: this.finalsPhaseCourts,
            firstFinalsRound: convertFirstFinalsRound(),
            thirdPlacePlayoff: this.currentFinalRound !== FirstFinalRound.Final && this.enableThirdPlacePlayoff,
            additionalPlayoffs: this.enableAdditionalPlayoffs
              ? this.additionalPlayoffPositions
                  .filter((position) => this.additionalPlayoffs[position].isEnabled)
                  .map(
                    (position): AdditionalPlayoffDto => ({
                      playoffPosition: position,
                      teamSelectorA: this.additionalPlayoffs[position].teamSelectorA,
                      teamSelectorB: this.additionalPlayoffs[position].teamSelectorB
                    })
                  )
              : []
          }
        : undefined;

    return {
      groups: this.groups.map(
        (group): ConfigureTournamentEndpointRequestGroupEntry => ({
          id: group.id ?? null,
          alphabeticalId: group.alphabeticalId,
          displayName: group.displayName.length === 0 ? null : group.displayName,
          teams: group.teams.map(
            (team): ConfigureTournamentEndpointRequestTeamEntry => ({
              id: team.id ?? null,
              name: team.teamLink === undefined ? team.name : null,
              teamLink:
                team.teamLink === undefined
                  ? null
                  : { planningRealmId: team.teamLink.planningRealmId, applicationTeamId: team.teamLink.applicationTeamId }
            })
          )
        })
      ),
      firstMatchKickoff: this.firstMatchKickoff.toISOString(),
      groupPhase: {
        schedule: {
          playTime: this.groupPhasePlayTime,
          pauseTime: this.groupPhasePauseTime
        },
        numberOfCourts: this.groupPhaseCourts,
        numberOfGroupRounds: this.groupPhaseRounds,
        useAlternatingOrder: this.groupPhaseAlternating
      },
      pauseBetweenGroupAndFinalsPhase: finalsPhaseConfig === undefined ? undefined : this.pauseBetweenGroupAndFinalsPhase,
      finalsPhase: finalsPhaseConfig
    };
  }
}
