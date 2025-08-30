import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
  selector: 'tp-delete-widget',
  templateUrl: './delete-widget.component.html',
  imports: [TranslateDirective, NgClass, FormsModule, ActionButtonComponent, TranslatePipe]
})
export class DeleteWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public thinLayout: boolean = false;

  @Output()
  public deleteClick = new EventEmitter<void>();

  protected confirmationText?: string;
  protected confirmationTextInput: string = '';
  protected allowDeletion = false;

  @Input()
  public set targetObjectName(value: string) {
    this.confirmationText = value.replace(/[^A-Za-z0-9.\-_|ÄÖÜäöüß ]+/g, '').trim();
  }

  protected checkConfirmationText(): void {
    this.allowDeletion = this.confirmationText !== undefined && this.confirmationText === this.confirmationTextInput.trim();
  }

  protected deleteClicked(): void {
    if (this.allowDeletion) {
      this.deleteClick.emit();
    }
  }
}
