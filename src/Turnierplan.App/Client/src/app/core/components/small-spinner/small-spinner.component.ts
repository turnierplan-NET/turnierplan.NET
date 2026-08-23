import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'tp-small-spinner',
  changeDetection: ChangeDetectionStrategy.Eager,
  templateUrl: './small-spinner.component.html'
})
export class SmallSpinnerComponent {
  @Input()
  public color: string = 'secondary';
}
