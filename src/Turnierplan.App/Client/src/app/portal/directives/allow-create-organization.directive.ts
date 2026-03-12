import { Directive, ElementRef, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';

import { AuthenticationService } from '../../core/services/authentication.service';

@Directive({ selector: '[tpAllowCreateOrganization]' })
export class AllowCreateOrganizationDirective implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly templateRef: TemplateRef<ElementRef>,
    private readonly viewContainer: ViewContainerRef,
    private readonly authenticationService: AuthenticationService
  ) {}

  public ngOnInit(): void {
    this.authenticationService
      .checkIfUserIsAllowedToCreateOrganization()
      .pipe(takeUntil(this.destroyed$))
      .subscribe({
        next: (allowCreateOrganization) => {
          this.viewContainer.clear();
          if (allowCreateOrganization) {
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
