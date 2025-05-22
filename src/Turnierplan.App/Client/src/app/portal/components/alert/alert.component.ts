import { Component, Input } from '@angular/core';

@Component({
  selector: 'tp-alert',
  templateUrl: './alert.component.html'
})
export class AlertComponent {
  @Input()
  public type: 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'info' = 'info';

  @Input()
  public icon?: string;

  @Input()
  public text?: string;

  @Input()
  public margin: string = 'm-0';
}
