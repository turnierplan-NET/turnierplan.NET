import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { showDeleteModal } from '../delete-modal/delete-modal.component';

@Component({
  selector: 'tp-delete-widget',
  templateUrl: './delete-widget.component.html',
  imports: [TranslateDirective, NgClass, FormsModule, ActionButtonComponent, TranslatePipe, E2eDirective]
})
export class DeleteWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public thinLayout: boolean = false;

  @Input()
  public set targetObjectName(value: string) {
    this._targetObjectName = value;
    this.confirmationText = value.replaceAll(/[^A-Za-z0-9.\-_|ÄÖÜäöüß ]+/g, '').trim();
  }

  @Output()
  public deleteClick = new EventEmitter<void>();

  protected confirmationText?: string;
  protected confirmationTextInput: string = '';
  protected allowDeletion = false;

  private _targetObjectName: string = '';

  constructor(private readonly modalService: NgbModal) {}

  protected checkConfirmationText(): void {
    this.allowDeletion = this.confirmationText !== undefined && this.confirmationText === this.confirmationTextInput.trim();
  }

  protected deleteClicked(): void {
    if (this.allowDeletion) {
      showDeleteModal(this.modalService, this.translationKey, this._targetObjectName).subscribe({
        next: (result): void => {
          if (result) {
            this.deleteClick.emit();
          }
        }
      });
    }
  }
}
