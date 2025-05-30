import { Component, Input } from '@angular/core';

export interface RankingView {
  position: number;
  team: string;
}

@Component({
  standalone: false,
  selector: 'tp-ranking',
  templateUrl: './ranking.component.html'
})
export class RankingComponent {
  @Input()
  public rankings: RankingView[] = [];
}
