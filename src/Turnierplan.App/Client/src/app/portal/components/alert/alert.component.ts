import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateDirective } from '@ngx-translate/core';

export type AlertType = 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'info';

@Component({
    selector: 'tp-alert',
    templateUrl: './alert.component.html',
    imports: [NgClass, TranslateDirective]
})
export class AlertComponent {
  @Input()
  public type: AlertType = 'info';

  @Input()
  public icon?: string;

  @Input()
  public text?: string;

  @Input()
  public textParams?: { [key: string]: unknown };

  @Input()
  public margin: string = 'm-0';
}
