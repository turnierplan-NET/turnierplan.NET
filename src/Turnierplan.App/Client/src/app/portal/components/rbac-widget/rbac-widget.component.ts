import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';
import { RbacOffcanvasComponent } from '../rbac-offcanvas/rbac-offcanvas.component';
import { NavigationStart, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs/operators';

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

  private offcanvas?: NgbOffcanvasRef;

  constructor(
    private readonly offcanvasService: NgbOffcanvas,
    private readonly router: Router
  ) {
    this.router.events
      .pipe(
        takeUntilDestroyed(),
        filter((event) => event instanceof NavigationStart)
      )
      .subscribe({
        next: () => {
          this.offcanvas?.close();
        }
      });
  }

  protected buttonClicked(): void {
    this.offcanvas = this.offcanvasService.open(RbacOffcanvasComponent, { position: 'end' });
    const component = this.offcanvas.componentInstance as RbacOffcanvasComponent;

    component.error$.subscribe({
      next: (value) => {
        this.offcanvas?.close();
        this.errorOccured.emit(value);
      }
    });

    component.setTarget(this.target);
  }
}
