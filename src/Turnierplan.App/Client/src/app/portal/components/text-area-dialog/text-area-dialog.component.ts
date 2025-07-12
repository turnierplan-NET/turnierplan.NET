import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  standalone: false,
  templateUrl: './text-area-dialog.component.html'
})
export class TextAreaDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected initialValue: string = '';
  protected currentValue: string = '';
  protected wasChanged = false;

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(translationKey: string, initialValue: string): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.initialValue = initialValue;
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

    if (sanitized !== sanitizedInitial) {
      this.modal.close(sanitized);
    } else {
      this.modal.dismiss();
    }
  }
}
