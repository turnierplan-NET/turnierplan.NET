import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  QueryList,
  SimpleChanges,
  ViewChild,
  ViewChildren
} from '@angular/core';

import { MatchType } from '../../../api';
import { GroupView } from '../groups/groups.component';
import { MatchView } from '../match-plan/match-plan.component';

type TreeColumn = {
  index: number;
  matches: MatchView[];
  dependencyIds: (number | undefined)[];
};

type ConnectingLine = {
  x1: number;
  y1: number;
  x2: number;
  y2: number;
};

@Component({
  selector: 'tp-match-tree',
  templateUrl: './match-tree.component.html',
  styleUrl: './match-tree.component.scss'
})
export class MatchTreeComponent implements OnChanges, AfterViewInit {
  @Input()
  public matches: MatchView[] = [];

  // IDEA: This should probably be changed so we don't reference a random interface from another component
  @Input()
  public groups: GroupView[] = [];

  @Output()
  public matchClick = new EventEmitter<number>();

  @ViewChild('containerDiv')
  public containerDiv!: ElementRef<HTMLDivElement>;

  @ViewChild('layoutedDiv')
  public layoutedDiv!: ElementRef<HTMLDivElement>;

  @ViewChild('additionalPaddingDiv')
  public additionalPaddingDiv!: ElementRef<HTMLDivElement>;

  @ViewChildren('matchTile', { read: ElementRef })
  public matchTiles!: QueryList<ElementRef<HTMLDivElement>>;

  protected containerClass = 'justify-content-center';
  protected columns: TreeColumn[] = [];
  protected rankingMatches: MatchView[] = [];
  protected showKickoffWithSeconds: boolean = false;
  protected connectingLinesSvgWidth = 0;
  protected connectingLinesSvgHeight = 0;
  protected connectingLines: ConnectingLine[] = [];

  public ngOnChanges(changes: SimpleChanges): void {
    if ('matches' in changes || 'groups' in changes) {
      this.columns = [];

      this.showKickoffWithSeconds = this.matches.some((match) => match.kickoff !== undefined && new Date(match.kickoff).getSeconds() !== 0);

      this.rankingMatches = this.matches.filter(
        (x) => x.type.matchType === MatchType.ThirdPlacePlayoff || x.type.matchType === MatchType.AdditionalPlayoff
      );
      this.rankingMatches.sort((a, b) => (a.type.playoffPosition ?? 0) - (b.type.playoffPosition ?? 0));

      const finalMatch = this.matches.find((x) => x.type.matchType === MatchType.Final);
      if (!finalMatch) {
        return;
      }

      this.columns.push({ index: 0, matches: [finalMatch], dependencyIds: [] });

      // By manually changing the team selectors of a match, you can create a situation where not every
      // match has a dependency on either 0 or 2 other matches. To accomodate this, undefined is used as a
      // "placeholder" id for this case.
      let dependantMatches: MatchView[] = [];
      let dependentMatchIds: (number | undefined)[] = [];

      // Prevent infinite loop in case the matches have a mutual dependency
      const includedMatchIds = [finalMatch.id];

      // eslint-disable-next-line no-constant-condition
      while (true) {
        const currentColumnMatches = this.columns.at(-1)!.matches;

        for (const match of currentColumnMatches) {
          for (const teamSelector of [match.teamSelectorKeyA, match.teamSelectorKeyB]) {
            if (!(teamSelector.charAt(0) === 'W' || teamSelector.charAt(0) === 'L')) {
              dependentMatchIds.push(undefined);
              continue;
            }

            const otherMatchIndex = +teamSelector.substring(1);
            const referredMatch = this.matches.find((match) => match.index === otherMatchIndex);

            if (!referredMatch || includedMatchIds.includes(referredMatch.id)) {
              dependentMatchIds.push(undefined);
              continue;
            }

            dependantMatches.push(referredMatch);
            dependentMatchIds.push(referredMatch.id);
          }
        }

        if (dependantMatches.length === 0) {
          break;
        }

        this.columns.at(-1)!.dependencyIds.push(...dependentMatchIds);
        this.columns.push({ index: this.columns.length, matches: [...dependantMatches], dependencyIds: [] });

        dependantMatches.forEach((match) => includedMatchIds.push(match.id));

        dependantMatches = [];
        dependentMatchIds = [];
      }

      this.columns.reverse();

      // setTimeout(...) to trigger change detection before recalculating
      setTimeout(() => this.recalculate());
    }
  }

  public ngAfterViewInit(): void {
    // setTimeout(...) to prevent 'Expression has changed after it was checked' error
    setTimeout(() => this.recalculate());
  }

  protected recalculate(): void {
    const containerDiv = this.containerDiv.nativeElement;

    const isContentBiggerThanViewport = containerDiv.scrollWidth > window.innerWidth;
    this.containerClass = isContentBiggerThanViewport ? 'justify-content-start' : 'justify-content-center';

    setTimeout(() => {
      // Code that relies on the positioning of the match tiles must be executed after an additional change
      // detection cycle. This is because the code above might change the positioning of the match tiles
      // due to a changing 'justify-content-...' class.

      const matchDivs = this.matchTiles.toArray().map((x) => x.nativeElement);

      this.positionRankingMatches(matchDivs);
      this.generateConnectingLines(matchDivs);

      this.connectingLinesSvgWidth = Math.max(...this.connectingLines.flatMap((l) => [l.x1, l.x2])) + 1;
      this.connectingLinesSvgHeight = Math.max(...this.connectingLines.flatMap((l) => [l.y1, l.y2])) + 1;
    });
  }

  private positionRankingMatches(matchDivs: HTMLDivElement[]): void {
    if (this.rankingMatches.length === 0) {
      return;
    }

    const finalMatch = this.matches.find((x) => x.type.matchType === MatchType.Final)!;
    const finalMatchDivId = `match-tile-${finalMatch.id}`;
    const finalMatchDiv = matchDivs.find((x) => x.id === finalMatchDivId);

    if (!finalMatchDiv) {
      return;
    }

    const paddingInPixels = 16;
    let nextMatchTileY = finalMatchDiv.offsetTop + finalMatchDiv.offsetHeight + paddingInPixels;

    for (const match of this.rankingMatches) {
      const matchDivId = `match-tile-${match.id}`;
      const matchDiv = matchDivs.find((x) => x.id === matchDivId);

      if (!matchDiv) {
        continue;
      }

      matchDiv.style.top = `${nextMatchTileY}px`;
      matchDiv.style.left = `${finalMatchDiv.offsetLeft}px`;

      // The div starts out with "display: none;". This is done to prevent flickering until the div is positioned
      matchDiv.style.display = 'block';

      nextMatchTileY += matchDiv.offsetHeight + paddingInPixels;
    }

    const missingSpaceY = Math.max(0.0, nextMatchTileY - this.layoutedDiv.nativeElement.offsetHeight);
    this.additionalPaddingDiv.nativeElement.style.height = `${missingSpaceY}px`;
  }

  private generateConnectingLines(matchDivs: HTMLDivElement[]): void {
    this.connectingLines = [];

    // Start at i=1 because the leftmost column has no visible dependencies
    for (let i = 1; i < this.columns.length; i++) {
      const column = this.columns[i];

      for (let j = 0; j < column.matches.length; j++) {
        const match = column.matches[j];

        this.addConnectingLines(matchDivs, match.id, column.dependencyIds[j * 2], column.dependencyIds[j * 2 + 1]);
      }
    }
  }

  private addConnectingLines(matchDivs: HTMLDivElement[], targetId?: number, dependencyId1?: number, dependencyId2?: number): void {
    const targetDivId = `match-tile-${targetId}`;
    const dependencyDivId1 = `match-tile-${dependencyId1}`;
    const dependencyDivId2 = `match-tile-${dependencyId2}`;

    const targetDiv = matchDivs.find((x) => x.id === targetDivId);
    const dependencyDiv1 = matchDivs.find((x) => x.id === dependencyDivId1);
    const dependencyDiv2 = matchDivs.find((x) => x.id === dependencyDivId2);

    if (targetDiv === undefined) {
      return;
    }

    const addIndividualLine = (dependencyDiv: HTMLDivElement, align: 'top' | 'center' | 'bottom'): void => {
      // Math.round(...) everything to ensure no lines appear thicker/thinner because of interpolation

      const yOffset = {
        top: -8,
        center: 0,
        bottom: 8
      }[align];

      const targetConnectorX = Math.round(targetDiv.offsetLeft);
      const targetConnectorY = Math.round(targetDiv.offsetTop + targetDiv.offsetHeight / 2 + yOffset);

      const dependencyConnectorX = Math.round(dependencyDiv.offsetLeft + dependencyDiv.offsetWidth);
      const dependencyConnectorY = Math.round(dependencyDiv.offsetTop + dependencyDiv.offsetHeight / 2);

      const middleX = Math.round((targetConnectorX + dependencyConnectorX) / 2);

      this.connectingLines.push({ x1: dependencyConnectorX, y1: dependencyConnectorY, x2: middleX, y2: dependencyConnectorY });
      this.connectingLines.push({ x1: middleX, y1: dependencyConnectorY, x2: middleX, y2: targetConnectorY });
      this.connectingLines.push({ x1: middleX, y1: targetConnectorY, x2: targetConnectorX, y2: targetConnectorY });
    };

    if (dependencyDiv1 !== undefined && dependencyDiv2 === undefined) {
      // only dependencyDiv1 is available
      addIndividualLine(dependencyDiv1, 'center');
    } else if (dependencyDiv1 === undefined && dependencyDiv2 !== undefined) {
      // only dependencyDiv2 is available
      addIndividualLine(dependencyDiv2, 'center');
    } else if (dependencyDiv1 !== undefined && dependencyDiv2 !== undefined) {
      // both dependencyDivs are available
      if (dependencyDiv1.offsetTop > dependencyDiv2.offsetTop) {
        // dependencyDiv1 is BELOW dependencyDiv2
        addIndividualLine(dependencyDiv1, 'bottom');
        addIndividualLine(dependencyDiv2, 'top');
      } else {
        addIndividualLine(dependencyDiv1, 'top');
        addIndividualLine(dependencyDiv2, 'bottom');
      }
    }
  }
}
