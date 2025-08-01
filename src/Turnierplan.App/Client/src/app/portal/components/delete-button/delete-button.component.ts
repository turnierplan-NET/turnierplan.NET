import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  standalone: false,
  selector: 'tp-delete-button',
  templateUrl: './delete-button.component.html'
})
export class DeleteButtonComponent {
  @Input()
  public reducedFootprint: boolean = false;

  @Input()
  public showLabel: boolean = false;

  @Output()
  public confirmed = new EventEmitter<void>();
}
