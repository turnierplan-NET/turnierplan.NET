import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';

import { MatchType, NullableOfMatchOutcomeType } from '../../../api';
import { GroupView } from '../groups/groups.component';
import { NgClass } from '@angular/common';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { LiveIndicatorComponent } from '../live-indicator/live-indicator.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';

// IDEA: This interface should probably be moved to a separate .ts file as long as it is referenced by other components
export interface MatchView {
  id: number;
  index: number;
  court: string;
  type: MatchViewType;
  kickoff?: Date;
  group: string;
  teamA: string;
  teamB: string;
  teamSelectorA: string;
  teamSelectorB: string;
  teamSelectorKeyA: string;
  teamSelectorKeyB: string;
  isLive: boolean;
  scoreA?: number;
  scoreB?: number;
  outcomeType?: NullableOfMatchOutcomeType;
  // IDEA: The properties below should probably be extracted from this interface
  scoreAccumulated?: number;
  showLoadingIndicator: boolean;
}

export interface MatchViewType {
  // IDEA: Find a more elegant solution for the properties below
  matchType: MatchType;
  displayName: string;
  playoffPosition?: number;
  hideOnMatchPlan: boolean;
}

interface MatchViewGrouping {
  title: MatchViewType;
  matches: MatchView[];
}

@Component({
  selector: 'tp-match-plan',
  templateUrl: './match-plan.component.html',
  styleUrls: ['./match-plan.component.scss'],
  imports: [NgClass, TranslateDirective, SmallSpinnerComponent, LiveIndicatorComponent, TranslatePipe, TranslateDatePipe]
})
export class MatchPlanComponent implements OnChanges {
  @Input()
  public matches: MatchView[] = [];

  // IDEA: This should probably be changed so we don't reference a random interface from another component
  @Input()
  public groups: GroupView[] = [];

  @Input()
  public allowReporting = false;

  @Input()
  public showAccumulatedScore = false;

  @Output()
  public matchClick = new EventEmitter<number>();

  protected groupedMatches: MatchViewGrouping[] = [];
  protected matchCount = 0;
  protected showKickoffWithSeconds: boolean = false;
  protected showCourtColumn: boolean = false;

  protected get columnCount(): number {
    return 6 + (this.showAccumulatedScore ? 1 : 0) + (this.showCourtColumn ? 1 : 0);
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if ('matches' in changes) {
      this.groupMatches();
    }
  }

  private groupMatches(): void {
    this.groupedMatches = [];
    this.matchCount = 0;
    this.showKickoffWithSeconds = false;

    let currentGroup: MatchViewGrouping = {
      title: {
        hideOnMatchPlan: false,
        matchType: MatchType.GroupMatch,
        displayName: ''
      },
      matches: []
    };

    this.matches.forEach((match) => {
      this.matchCount++;
      const isCorrectGroup = currentGroup.title.displayName === match.type.displayName;

      if (isCorrectGroup) {
        currentGroup.matches.push(match);
      } else {
        this.groupedMatches.push(currentGroup);
        currentGroup = {
          title: match.type,
          matches: [match]
        };
      }

      if (match.kickoff !== undefined && new Date(match.kickoff).getSeconds() !== 0) {
        this.showKickoffWithSeconds = true;
      }
    });

    this.showCourtColumn = new Set(this.matches.map((x) => x.court)).size > 1;

    if (currentGroup.matches.length > 0) {
      this.groupedMatches.push(currentGroup);
    }
  }
}
