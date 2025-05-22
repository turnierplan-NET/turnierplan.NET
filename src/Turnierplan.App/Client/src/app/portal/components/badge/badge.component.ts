import { Component, Input } from '@angular/core';

@Component({
  selector: 'tp-badge',
  templateUrl: './badge.component.html'
})
export class BadgeComponent {
  @Input()
  public context!: string;

  @Input()
  public label!: string;

  @Input()
  public value!: string | number;
}
