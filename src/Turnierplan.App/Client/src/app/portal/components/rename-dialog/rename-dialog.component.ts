import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
  templateUrl: './rename-dialog.component.html',
  imports: [TranslateDirective, FormsModule, NgClass, ActionButtonComponent]
})
export class RenameDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected allowReset = false;
  protected initialValue?: string;
  protected currentValue: string = '';
  protected wasChanged = false;
  protected showError?: 'RequiredFeedback';

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(translationKey: string, allowReset: boolean, initialValue?: string): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.allowReset = allowReset;
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

    if (sanitized.length > 0) {
      if (sanitized !== sanitizedInitial) {
        this.modal.close(sanitized);
      } else {
        this.modal.dismiss();
      }
    } else if (sanitized.length === 0 && this.allowReset) {
      if (sanitizedInitial.length > 0) {
        this.modal.close();
      } else {
        this.modal.dismiss();
      }
    } else {
      // In the case that the value is empty but reset is not allowed, display error
      this.showError = 'RequiredFeedback';
    }
  }
}
