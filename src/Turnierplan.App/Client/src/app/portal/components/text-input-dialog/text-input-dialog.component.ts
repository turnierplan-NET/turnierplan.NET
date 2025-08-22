import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertType } from '../alert/alert.component';

export interface TextInputDialogAlert {
  type: AlertType;
  icon: string;
}

@Component({
  standalone: false,
  templateUrl: './text-input-dialog.component.html'
})
export class TextInputDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected alert?: TextInputDialogAlert;
  protected initialValue: string = '';
  protected textArea: boolean = false;
  protected isRequired: boolean = false;
  protected currentValue: string = '';
  protected wasChanged = false;
  protected showError?: 'RequiredFeedback';

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(translationKey: string, initialValue: string, textArea: boolean, isRequired: boolean, alert?: TextInputDialogAlert): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.alert = alert;
    this.initialValue = initialValue;
    this.textArea = textArea;
    this.isRequired = isRequired;
    this.currentValue = initialValue;
    this.wasChanged = false;

    this.isInitialized = true;
  }

  protected save(isKeyupEvent: boolean = false): void {
    if (isKeyupEvent && !this.wasChanged) {
      // When the dialog is opened by pressing Enter, the release event should not immediately trigger saving.
      return;
    }

    const sanitized = this.currentValue.trim();
    const sanitizedInitial = (this.initialValue ?? '').trim();

    if (sanitized.length === 0 && this.isRequired) {
      this.showError = 'RequiredFeedback';
      return;
    }

    if (sanitized !== sanitizedInitial) {
      this.modal.close(sanitized);
    } else {
      this.modal.dismiss();
    }
  }
}
