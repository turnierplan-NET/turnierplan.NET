import { Component, inject, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'tp-offcanvas-wrapper',
  imports: [ActionButtonComponent],
  templateUrl: './offcanvas-wrapper.component.html'
})
export class OffcanvasWrapperComponent implements OnDestroy {
  @ViewChild('offcanvasTemplate')
  protected offcanvasTemplate!: TemplateRef<any>;

  private readonly offcanvasService = inject(NgbOffcanvas);
  private offcanvasRef?: NgbOffcanvasRef;

  public get isOpen(): boolean {
    return this.offcanvasRef !== undefined;
  }

  public ngOnDestroy(): void {
    // Ensure the offcanvas is closed when this component is destroyed
    this.close();
  }

  public show(): void {
    if (this.offcanvasRef) {
      return;
    }

    this.offcanvasRef = this.offcanvasService.open(this.offcanvasTemplate, { position: 'end' });

    this.offcanvasRef.hidden.subscribe({
      next: () => {
        this.offcanvasRef = undefined;
      }
    });
  }

  public close(): void {
    if (!this.offcanvasRef) {
      return;
    }

    // This will cause the 'hidden' observable to emit which will set the variable to undefined
    this.offcanvasRef.close();
  }
}
