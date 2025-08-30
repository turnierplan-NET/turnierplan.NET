import { Component, Input } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';

@Component({
    selector: 'tp-badge',
    templateUrl: './badge.component.html',
    imports: [TranslateDirective]
})
export class BadgeComponent {
  @Input()
  public context!: string;

  @Input()
  public label!: string;

  @Input()
  public value!: string | number;
}
