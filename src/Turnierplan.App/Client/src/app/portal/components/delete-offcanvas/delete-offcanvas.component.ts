import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { DeleteWidgetComponent } from '../delete-widget/delete-widget.component';
import { TranslateDirective } from '@ngx-translate/core';
import { OffcanvasWrapperComponent } from '../offcanvas-wrapper/offcanvas-wrapper.component';

@Component({
  selector: 'tp-delete-offcanvas',
  imports: [DeleteWidgetComponent, TranslateDirective, OffcanvasWrapperComponent],
  templateUrl: './delete-offcanvas.component.html'
})
export class DeleteOffcanvasComponent {
  @Input()
  public translationKey: string = '';

  @Output()
  public deleteClick = new EventEmitter<string>();

  @ViewChild('offcanvasWrapper')
  protected offcanvasWrapper!: OffcanvasWrapperComponent;

  protected targetObjectName?: string;
  protected targetObjectId?: string;

  public show(targetObjectId: string, targetObjectName: string): void {
    if (this.offcanvasWrapper.isOpen) {
      return;
    }

    this.targetObjectName = targetObjectName;
    this.targetObjectId = targetObjectId;

    this.offcanvasWrapper.show();
  }

  protected confirmed(): void {
    this.offcanvasWrapper.close();

    if (this.targetObjectId) {
      this.deleteClick.emit(this.targetObjectId);
    }
  }
}
