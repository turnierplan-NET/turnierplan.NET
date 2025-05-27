import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'tp-loading-indicator',
  templateUrl: './loading-indicator.component.html',
  styleUrls: ['./loading-indicator.component.scss']
})
export class LoadingIndicatorComponent {
  @Input()
  public center = true;

  @Input()
  public marginY = true;
}
