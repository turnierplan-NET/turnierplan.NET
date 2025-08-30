import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'tp-tooltip-icon',
    templateUrl: './tooltip-icon.component.html',
    imports: [NgClass, NgbTooltip, TranslatePipe]
})
export class TooltipIconComponent {
  @Input()
  public icon: string = 'info-circle';

  @Input()
  public margin: boolean = true;

  @Input()
  public iconClass: string = '';

  @Input()
  public tooltipText: string = '';

  @Input()
  public tooltipTextParams: { [key: string]: unknown } = {};

  protected get computedClasses(): string[] {
    return [`bi bi-${this.icon}`, this.iconClass, this.margin ? 'mx-2' : ''];
  }
}
