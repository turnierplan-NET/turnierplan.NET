import { Directive, ElementRef, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';

import { AuthenticationService } from '../../../core/services/authentication.service';

@Directive({ selector: '[tpIsAdministrator]' })
export class IsAdministratorDirective implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly templateRef: TemplateRef<ElementRef>,
    private readonly viewContainer: ViewContainerRef,
    private readonly authenticationService: AuthenticationService
  ) {}

  public ngOnInit(): void {
    this.authenticationService
      .checkIfUserIsAdministrator()
      .pipe(takeUntil(this.destroyed$))
      .subscribe({
        next: (isAdministrator) => {
          this.viewContainer.clear();
          if (isAdministrator) {
            this.viewContainer.createEmbeddedView(this.templateRef);
          }
        }
      });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
