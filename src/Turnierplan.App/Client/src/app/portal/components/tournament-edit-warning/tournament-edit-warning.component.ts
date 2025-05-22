import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'tp-tournament-edit-warning',
  templateUrl: './tournament-edit-warning.component.html'
})
export class TournamentEditWarningComponent {
  @Input()
  public translationKey!: string;

  @Input()
  public accepted: boolean = false;

  @Output()
  public acceptedChange = new EventEmitter<boolean>();
}
