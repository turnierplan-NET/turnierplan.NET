import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AlertComponent } from '../alert/alert.component';
import { TranslateDirective } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'tp-tournament-edit-warning',
  templateUrl: './tournament-edit-warning.component.html',
  imports: [AlertComponent, TranslateDirective, FormsModule]
})
export class TournamentEditWarningComponent {
  @Input()
  public translationKey!: string;

  @Input()
  public textParameters: { [key: string]: any } = {};

  @Input()
  public accepted: boolean = false;

  @Output()
  public acceptedChange = new EventEmitter<boolean>();
}
