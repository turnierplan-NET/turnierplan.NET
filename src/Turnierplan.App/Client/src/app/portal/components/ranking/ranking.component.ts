import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { RankingReason } from '../../../api/models/ranking-reason';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';

export interface RankingView {
  position: number;
  isHidden: boolean;
  rankingOverwriteId?: number;
  team?: string;
  reason?: RankingReason;
}

@Component({
  selector: 'tp-ranking',
  templateUrl: './ranking.component.html',
  imports: [TranslateDirective, TranslatePipe, E2eDirective, DeleteButtonComponent, TooltipIconComponent]
})
export class RankingComponent {
  @Input()
  public rankings: RankingView[] = [];

  @Input()
  public allowRemoveOverwrite: boolean = false;

  @Output()
  public removeRankingOverwrite = new EventEmitter<number>();

  protected readonly rankingReason = RankingReason;

  protected get hasRankingOverwrites(): boolean {
    return this.rankings.some((x) => x.rankingOverwriteId !== undefined);
  }
}
