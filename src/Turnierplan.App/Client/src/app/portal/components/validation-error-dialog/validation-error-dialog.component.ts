import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: './validation-error-dialog.component.html'
})
export class ValidationErrorDialogComponent {
  protected _errors: string[] = [];

  constructor(protected readonly modal: NgbActiveModal) {}

  public set errors(value: string[]) {
    this._errors = value;
  }
}
