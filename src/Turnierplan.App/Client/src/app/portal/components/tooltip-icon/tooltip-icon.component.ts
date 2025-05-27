import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'tp-tooltip-icon',
  templateUrl: './tooltip-icon.component.html'
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
