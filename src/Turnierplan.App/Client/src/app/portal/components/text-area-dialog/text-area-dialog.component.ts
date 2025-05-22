import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  templateUrl: './text-area-dialog.component.html'
})
export class TextAreaDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected maxLength: number = 0;
  protected initialValue: string = '';
  protected currentValue: string = '';
  protected wasChanged = false;

  constructor(protected readonly modal: NgbActiveModal) {}

  protected get currentLength(): number {
    return this.currentValue.trim().length;
  }

  public init(translationKey: string, maxLength: number, initialValue: string): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.maxLength = maxLength;
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

    if (sanitized.length > this.maxLength) {
      return;
    }

    const sanitizedInitial = (this.initialValue ?? '').trim();

    if (sanitized !== sanitizedInitial) {
      this.modal.close(sanitized);
    } else {
      this.modal.dismiss();
    }
  }
}
