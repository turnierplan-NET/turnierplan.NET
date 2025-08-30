import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  selector: 'tp-loading-indicator',
  templateUrl: './loading-indicator.component.html',
  styleUrls: ['./loading-indicator.component.scss'],
  imports: [NgClass]
})
export class LoadingIndicatorComponent {
  @Input()
  public center = true;

  @Input()
  public marginY = true;
}
