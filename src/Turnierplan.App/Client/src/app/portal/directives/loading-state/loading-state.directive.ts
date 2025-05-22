import { Directive, ElementRef, Input, TemplateRef, ViewContainerRef } from '@angular/core';

import { LoadingErrorComponent } from '../../components/loading-error/loading-error.component';
import { LoadingIndicatorComponent } from '../../components/loading-indicator/loading-indicator.component';
import { TitleService } from '../../services/title.service';

export interface LoadingState {
  isLoading: boolean;
  error?: unknown;
}

@Directive({
  selector: '[tpLoadingState]'
})
export class LoadingStateDirective {
  constructor(
    private readonly templateRef: TemplateRef<ElementRef>,
    private readonly viewContainer: ViewContainerRef,
    private readonly titleService: TitleService
  ) {}

  @Input()
  public set tpLoadingState(state: LoadingState) {
    this.viewContainer.clear();

    if (state.isLoading) {
      this.viewContainer.createComponent(LoadingIndicatorComponent);
    } else if (state.error !== undefined) {
      this.titleService.setTitleTranslated('Portal.ErrorPage.Title');
      const component = this.viewContainer.createComponent(LoadingErrorComponent);
      component.instance.error = state.error;
    } else {
      this.viewContainer.createEmbeddedView(this.templateRef);
    }
  }
}
