import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { NgbModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { NgClass } from '@angular/common';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { showDeleteModal } from '../delete-modal/delete-modal.component';

@Component({
  selector: 'tp-delete-button',
  templateUrl: './delete-button.component.html',
  imports: [NgbPopover, NgClass, TranslateDirective, TranslatePipe]
})
export class DeleteButtonComponent {
  @Input()
  public reducedFootprint: boolean = false;

  @Input()
  public showLabel: boolean = false;

  @Input()
  public disabled: boolean = false;

  @Input()
  public modalConfirmation: boolean = false;

  @Input()
  public translationKey?: string;

  @Input()
  public targetObjectName?: string;

  @Output()
  public confirmed = new EventEmitter<void>();

  private readonly modalService = inject(NgbModal);

  protected deleteClicked(): void {
    if (this.modalConfirmation) {
      if (!this.translationKey || !this.targetObjectName) {
        throw new Error('The translation key and target object name must be specified if modal confirmation is enabled.');
      }

      showDeleteModal(this.modalService, this.translationKey, this.targetObjectName).subscribe({
        next: (result): void => {
          if (result) {
            this.confirmed.emit();
          }
        }
      });
    } else {
      this.confirmed.emit();
    }
  }
}
