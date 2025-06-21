import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { RbacOffcanvasComponent } from '../rbac-offcanvas/rbac-offcanvas.component';

interface IRbacWidgetTarget {
  name: string;
  rbacScopeId: string;
}

@Component({
  standalone: false,
  selector: 'tp-rbac-widget',
  templateUrl: './rbac-widget.component.html'
})
export class RbacWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public target!: IRbacWidgetTarget;

  @Input()
  public buttonOnly: boolean = false;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  constructor(private readonly offcanvasService: NgbOffcanvas) {}

  protected buttonClicked(): void {
    const ref = this.offcanvasService.open(RbacOffcanvasComponent, { position: 'end' });
    const component = ref.componentInstance as RbacOffcanvasComponent;

    component.error$.subscribe({
      next: (value) => {
        ref.close();
        this.errorOccured.emit(value);
      }
    });

    component.setTarget(this.target);
  }
}
