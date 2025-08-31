import { Directive, ElementRef, Input, OnDestroy, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AuthorizationService } from '../../core/services/authorization.service';
import { Action } from '../../generated/actions';

type Config = [string, Action];

@Directive({ selector: '[tpIsActionAllowed]' })
export class IsActionAllowedDirective implements OnInit, OnDestroy {
  private readonly destroyed$ = new Subject<void>();
  private config?: Config;

  constructor(
    private readonly templateRef: TemplateRef<ElementRef>,
    private readonly viewContainer: ViewContainerRef,
    private readonly authorizationService: AuthorizationService
  ) {}

  @Input()
  public set tpIsActionAllowed(value: Config | undefined) {
    this.config = value;
  }

  public ngOnInit(): void {
    if (!this.config) {
      this.viewContainer.clear();
      this.viewContainer.createEmbeddedView(this.templateRef);
      return;
    }

    this.authorizationService
      .isActionAllowed$(this.config[0], this.config[1])
      .pipe(takeUntil(this.destroyed$))
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
