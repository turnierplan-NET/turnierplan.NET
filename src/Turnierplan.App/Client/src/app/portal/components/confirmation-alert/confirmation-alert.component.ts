import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AlertComponent } from '../alert/alert.component';
import { TranslateDirective } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'tp-confirmation-alert',
  templateUrl: './confirmation-alert.component.html',
  imports: [AlertComponent, TranslateDirective, FormsModule]
})
export class ConfirmationAlertComponent {
  @Input()
  public translationKey!: string;

  @Input()
  public textParameters: { [key: string]: any } = {};

  @Input()
  public accepted: boolean = false;

  @Output()
  public acceptedChange = new EventEmitter<boolean>();
}
