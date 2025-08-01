import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  standalone: false,
  selector: 'tp-unsaved-changes-alert',
  templateUrl: './unsaved-changes-alert.component.html'
})
export class UnsavedChangesAlertComponent {
  @Input()
  public inProgress: boolean = false;

  @Output()
  public save = new EventEmitter<void>();
}
