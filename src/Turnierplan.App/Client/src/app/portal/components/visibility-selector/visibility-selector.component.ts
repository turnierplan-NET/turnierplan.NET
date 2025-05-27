import { Component, EventEmitter, Input, Output } from '@angular/core';

import { Visibility } from '../../../api';

@Component({
  standalone: false,
  selector: 'tp-visibility-selector',
  templateUrl: './visibility-selector.component.html'
})
export class VisibilitySelectorComponent {
  @Input()
  public visibility!: Visibility;

  @Output()
  public visibilityChange = new EventEmitter<Visibility>();

  protected readonly Visibility = Visibility;

  protected setVisibility(value: Visibility): void {
    this.visibility = value;
    this.visibilityChange.emit(value);
  }
}
