import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { RenameDialogComponent } from '../rename-dialog/rename-dialog.component';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
    selector: 'tp-rename-button',
    templateUrl: './rename-button.component.html',
    imports: [ActionButtonComponent]
})
export class RenameButtonComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public displayLabel = true;

  @Input()
  public current?: string;

  @Input()
  public allowReset = false;

  @Input()
  public disabled = false;

  @Output()
  public renamed = new EventEmitter<string>();

  @Output()
  public resetName = new EventEmitter<void>();

  constructor(private readonly modalService: NgbModal) {}

  protected openRenameDialog(): void {
    const ref = this.modalService.open(RenameDialogComponent, {
      centered: true
    });

    const component = ref.componentInstance as RenameDialogComponent;
    component.init(this.translationKey, this.allowReset, this.current);

    ref.closed.subscribe({
      next: (value: string) => {
        if (value && value.length > 0) {
          this.renamed.emit(value);
        } else if (this.allowReset) {
          this.resetName.emit();
        }
      }
    });
  }
}
