import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-action-button',
  templateUrl: './action-button.component.html',
  imports: [NgClass, TranslateDirective, TranslatePipe]
})
export class ActionButtonComponent {
  @Input()
  public icon?: string;

  @Input()
  public type = 'outline-primary';

  @Input()
  public title = '';

  @Input()
  public titleParams: { [key: string]: unknown } = {};

  @Input()
  public mode: 'IconLeftAndText' | 'IconRightAndText' | 'IconOnly' = 'IconLeftAndText';

  @Input()
  public disabled = false;

  @Output()
  public buttonClick = new EventEmitter<void>();

  // TODO: Replace btn-outline-dark with btn-outline-secondary (or light btn?) if dark mode is active
}
