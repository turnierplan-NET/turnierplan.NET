import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
  templateUrl: './validation-error-dialog.component.html',
  imports: [TranslateDirective, ActionButtonComponent]
})
export class ValidationErrorDialogComponent {
  protected _errors: { [key: string]: string[] } = {};
  protected _errorKeys: string[] = [];

  constructor(protected readonly modal: NgbActiveModal) {}

  public set errors(value: { [key: string]: string[] }) {
    this._errors = value;
    this._errorKeys = Object.keys(value);
  }
}
