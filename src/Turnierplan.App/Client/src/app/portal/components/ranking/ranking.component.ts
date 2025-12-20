import { Component, Input } from '@angular/core';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { RankingReason } from '../../../api/models/ranking-reason';

export interface RankingView {
  position: number;
  team: string;
  reason: RankingReason;
}

@Component({
  selector: 'tp-ranking',
  templateUrl: './ranking.component.html',
  imports: [TranslateDirective, TranslatePipe, E2eDirective]
})
export class RankingComponent {
  @Input()
  public rankings: RankingView[] = [];
}
