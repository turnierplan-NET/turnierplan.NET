import { Component, Input } from '@angular/core';

@Component({
  selector: 'tp-action-button',
  templateUrl: './action-button.component.html'
})
export class ActionButtonComponent {
  @Input()
  public icon?: string;

  @Input()
  public type = 'outline-primary';

  @Input()
  public title = '';

  @Input()
  public mode: 'IconLeftAndText' | 'IconRightAndText' | 'IconOnly' = 'IconLeftAndText';

  @Input()
  public disabled = false;
}
