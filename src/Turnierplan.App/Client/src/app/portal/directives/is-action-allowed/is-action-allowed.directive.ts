import { Directive, ElementRef, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { Action } from '../../../generated/actions';
import { map } from 'rxjs/operators';

type Config = [string, Action];

@Directive({
  standalone: false,
  selector: '[tpIsActionAllowed]'
})
export class IsActionAllowedDirective implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();
  private config?: Config;

  constructor(
    private readonly templateRef: TemplateRef<ElementRef>,
    private readonly viewContainer: ViewContainerRef,
    private readonly authorizationService: AuthorizationService
  ) {}

  @Input()
  public set tpIsActionAllowed(value: Config) {
    this.config = value;
  }

  public ngOnInit(): void {
    this.authorizationService
      .getRoles$(this.config![0])
      .pipe(
        takeUntil(this.destroyed$),
        map((availableRoles) => this.config![1].isAllowed(availableRoles))
      )
      .subscribe({
        next: (isAllowed) => {
          this.viewContainer.clear();
          if (isAllowed) {
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
