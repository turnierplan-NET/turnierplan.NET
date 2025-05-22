import { Directive, ElementRef, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { ReplaySubject, Subject, switchMap, takeUntil } from 'rxjs';

import { AuthenticationService } from '../../../core/services/authentication.service';

@Directive({
  selector: '[tpHasRole]'
})
export class HasRoleDirective implements OnInit, OnDestroy {
  private readonly roleId$ = new ReplaySubject<string>(1);
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly templateRef: TemplateRef<ElementRef>,
    private readonly viewContainer: ViewContainerRef,
    private readonly authenticationService: AuthenticationService
  ) {}

  @Input()
  public set tpHasRole(roleId: string) {
    this.roleId$.next(roleId);
  }

  public ngOnInit(): void {
    this.roleId$
      .pipe(
        switchMap((roleId) => this.authenticationService.checkIfUserHasRole(roleId)),
        takeUntil(this.destroyed$)
      )
      .subscribe({
        next: (hasRole) => {
          this.viewContainer.clear();
          if (hasRole) {
            this.viewContainer.createEmbeddedView(this.templateRef);
          }
        }
      });
  }

  public ngOnDestroy(): void {
    this.roleId$.complete();
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
