import { Component, Input } from '@angular/core';

@Component({
  selector: 'tp-small-spinner',
  templateUrl: './small-spinner.component.html'
})
export class SmallSpinnerComponent {
  @Input()
  public color: string = 'secondary';
}
