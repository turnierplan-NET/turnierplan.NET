import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: './rename-dialog.component.html'
})
export class RenameDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected allowReset = false;
  protected maxLength?: number;
  protected initialValue?: string;
  protected currentValue: string = '';
  protected wasChanged = false;

  protected showError?: 'RequiredFeedback' | 'MaxLengthFeedback';

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(translationKey: string, allowReset: boolean, maxLength?: number, initialValue?: string): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.allowReset = allowReset;
    this.maxLength = maxLength;
    this.initialValue = initialValue;
    this.currentValue = initialValue ?? '';
    this.wasChanged = false;
    this.showError = undefined;

    this.isInitialized = true;
  }

  protected save(isKeyupEvent: boolean = false): void {
    if (isKeyupEvent && !this.wasChanged) {
      // When the dialog is opened by pressing Enter, the release event should not immediately trigger saving.
      return;
    }

    const sanitized = this.currentValue.trim();
    const sanitizedInitial = (this.initialValue ?? '').trim();

    if (this.maxLength !== undefined && sanitized.length > this.maxLength) {
      this.showError = 'MaxLengthFeedback';
      return;
    }

    if (sanitized.length > 0) {
      if (sanitized !== sanitizedInitial) {
        this.modal.close(sanitized);
      } else {
        this.modal.dismiss();
      }
    } else if (sanitized.length === 0 && this.allowReset) {
      if (sanitizedInitial.length > 0) {
        this.modal.close(undefined);
      } else {
        this.modal.dismiss();
      }
    } else {
      // In the case that the value is empty but reset is not allowed, display error
      this.showError = 'RequiredFeedback';
    }
  }
}
