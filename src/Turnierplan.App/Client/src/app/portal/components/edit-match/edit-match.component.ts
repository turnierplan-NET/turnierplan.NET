import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

import { MatchState, MatchType, NullableOfMatchOutcomeType, SetMatchOutcomeEndpointRequest } from '../../../api';
import { GroupView } from '../groups/groups.component';
import { MatchView } from '../match-plan/match-plan.component';

@Component({
  standalone: false,
  templateUrl: './edit-match.component.html',
  styleUrls: ['./edit-match.component.scss']
})
export class EditMatchComponent implements OnDestroy, AfterViewInit {
  @ViewChild('inputScoreA')
  public inputScoreA!: ElementRef<HTMLInputElement>;

  public readonly onSubmit$ = new Subject<SetMatchOutcomeEndpointRequest>();

  protected readonly matchOutcomeTypes = [
    NullableOfMatchOutcomeType.Standard,
    NullableOfMatchOutcomeType.AfterOvertime,
    NullableOfMatchOutcomeType.AfterPenalties,
    NullableOfMatchOutcomeType.SpecialScoring
  ];

  protected match?: MatchView;
  protected isDrawAllowed = true;
  protected displayOverwriteNotice = false;
  protected displayDrawNotAllowedNotice = false;

  // IDEA: This should probably be changed so we don't reference a random interface from another component
  protected groups: GroupView[] = [];

  protected scoreTeamA: number = 0;
  protected scoreTeamB: number = 0;
  protected outcomeType: NullableOfMatchOutcomeType = NullableOfMatchOutcomeType.Standard;

  constructor(protected readonly modal: NgbActiveModal) {}

  public ngOnDestroy(): void {
    if (!this.onSubmit$.closed) {
      this.onSubmit$.complete();
    }
  }

  public ngAfterViewInit(): void {
    this.inputScoreA.nativeElement.focus();
    setTimeout(() => this.inputScoreA.nativeElement.select(), 20);
  }

  public initialize(match: MatchView, groups: GroupView[] = []): void {
    this.match = match;
    this.groups = groups;
    this.isDrawAllowed = match.type.matchType === MatchType.GroupMatch;
    this.displayOverwriteNotice =
      !match.isLive && (match.scoreA !== undefined || match.scoreB !== undefined || match.outcomeType !== undefined);
    this.scoreTeamA = match.scoreA ?? 0;
    this.scoreTeamB = match.scoreB ?? 0;
    this.outcomeType = match.outcomeType ?? NullableOfMatchOutcomeType.Standard;
  }

  protected closeModal(action: 'cancel' | 'clear' | 'save' | 'saveLive'): void {
    const isValidScore = (value: number): boolean => !isNaN(value) && value === parseInt(`${value}`, 10);

    if (action !== 'cancel') {
      let request: SetMatchOutcomeEndpointRequest;

      switch (action) {
        case 'clear':
          request = { state: MatchState.NotStarted };

          break;
        case 'save':
          if (!this.isDrawAllowed && this.scoreTeamA === this.scoreTeamB) {
            this.displayDrawNotAllowedNotice = true;
            return;
          }

          if (!isValidScore(this.scoreTeamA) || !isValidScore(this.scoreTeamB)) {
            return;
          }

          request = {
            state: MatchState.Finished,
            scoreA: this.scoreTeamA,
            scoreB: this.scoreTeamB,
            outcomeType: this.outcomeType
          };

          break;
        case 'saveLive':
          if (!isValidScore(this.scoreTeamA) || !isValidScore(this.scoreTeamB)) {
            return;
          }

          request = {
            state: MatchState.CurrentlyPlaying,
            scoreA: this.scoreTeamA,
            scoreB: this.scoreTeamB,
            outcomeType: this.outcomeType
          };

          break;
      }

      this.onSubmit$.next(request);
    }

    this.modal.close();
  }
}
