import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

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
    this.confirmationText = value.replace(/[^A-Za-z0-9.\-_|ÄÖÜäöüß ]+/g, '').trim();
  }

  @Output()
  public deleteClick = new EventEmitter<void>();

  protected confirmationText?: string;
  protected confirmationTextInput: string = '';
  protected allowDeletion = false;
  protected openModal?: NgbModalRef;

  constructor(private readonly modalService: NgbModal) {}

  protected checkConfirmationText(): void {
    this.allowDeletion = this.confirmationText !== undefined && this.confirmationText === this.confirmationTextInput.trim();
  }

  protected deleteClicked(template: TemplateRef<unknown>): void {
    if (this.allowDeletion) {
      this.openModal = this.modalService.open(template, {
        size: 'md',
        fullscreen: 'md',
        centered: true
      });
    }
  }

  protected confirmDeleteClicked(): void {
    this.openModal?.dismiss();
    this.deleteClick.emit();
  }
}
