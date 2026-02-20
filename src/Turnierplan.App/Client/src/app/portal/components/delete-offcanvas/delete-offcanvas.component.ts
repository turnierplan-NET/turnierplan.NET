import { Component, EventEmitter, inject, Input, Output, TemplateRef, ViewChild } from '@angular/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { DeleteWidgetComponent } from '../delete-widget/delete-widget.component';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'tp-delete-offcanvas',
  imports: [ActionButtonComponent, DeleteWidgetComponent, TranslateDirective],
  templateUrl: './delete-offcanvas.component.html',
  styleUrl: './delete-offcanvas.component.scss'
})
export class DeleteOffcanvasComponent {
  @Input()
  public translationKey: string = '';

  @Output()
  public deleteClick = new EventEmitter<string>();

  @ViewChild('offcanvasTemplate')
  protected offcanvasTemplate!: TemplateRef<any>;

  protected targetObjectName?: string;
  protected targetObjectId?: string;

  private readonly offcanvasService = inject(NgbOffcanvas);
  private currentOffcanvas?: NgbOffcanvasRef;

  public show(targetObjectId: string, targetObjectName: string): void {
    if (this.currentOffcanvas) {
      return;
    }

    this.targetObjectName = targetObjectName;
    this.targetObjectId = targetObjectId;

    this.currentOffcanvas = this.offcanvasService.open(this.offcanvasTemplate, { position: 'end' });
  }

  public close(): void {
    this.currentOffcanvas?.close();
    this.currentOffcanvas = undefined;
    this.targetObjectId = undefined;
    this.targetObjectName = undefined;
  }

  protected confirmed(): void {
    // Remember the id because close() will reset the field
    const id = this.targetObjectId;

    this.close();

    if (!id) {
      return;
    }
    this.deleteClick.emit(id);
  }

  // TODO: Close currently open offcanvas on confirm/abort
}
