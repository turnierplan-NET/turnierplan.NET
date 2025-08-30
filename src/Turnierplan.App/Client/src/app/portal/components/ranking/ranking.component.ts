import { Component, Input } from '@angular/core';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';

export interface RankingView {
  position: number;
  team: string;
}

@Component({
    selector: 'tp-ranking',
    templateUrl: './ranking.component.html',
    imports: [TranslateDirective, TranslatePipe]
})
export class RankingComponent {
  @Input()
  public rankings: RankingView[] = [];
}
