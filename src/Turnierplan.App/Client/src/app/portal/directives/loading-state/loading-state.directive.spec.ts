import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { ErrorPageComponent } from '../../components/error-page/error-page.component';
import { IllustrationComponent } from '../../components/illustration/illustration.component';
import { LoadingErrorComponent } from '../../components/loading-error/loading-error.component';
import { LoadingIndicatorComponent } from '../../components/loading-indicator/loading-indicator.component';
import { TitleService } from '../../services/title.service';

import { LoadingStateDirective } from './loading-state.directive';

@Component({
  standalone: false,
  selector: 'tp-is-loading-true-test-component',
  template: `<ng-container *tpLoadingState="{ isLoading: true }">
    <p>Hello World</p>
  </ng-container>`
})
class IsLoadingTrueTestComponent {}

@Component({
  standalone: false,
  selector: 'tp-is-loading-false-test-component',
  template: `<ng-container *tpLoadingState="{ isLoading: false }">
    <p>Hello World</p>
  </ng-container>`
})
class IsLoadingFalseTestComponent {}

@Component({
  standalone: false,
  selector: 'tp-is-loading-false-with-404-error-test-component',
  template: `<ng-container *tpLoadingState="{ isLoading: false, error: { status: 404 } }">
    <p>Hello World</p>
  </ng-container>`
})
class IsLoadingFalseWith404ErrorTestComponent {}

@Component({
  standalone: false,
  selector: 'tp-is-loading-false-with-500-error-test-component',
  template: `<ng-container *tpLoadingState="{ isLoading: false, error: { status: 500 } }">
    <p>Hello World</p>
  </ng-container>`
})
class IsLoadingFalseWith500ErrorTestComponent {}

describe('LoadingStateDirective', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [
        LoadingStateDirective,
        LoadingErrorComponent,
        IsLoadingTrueTestComponent,
        IsLoadingFalseTestComponent,
        IsLoadingFalseWith404ErrorTestComponent,
        IsLoadingFalseWith500ErrorTestComponent,
        ErrorPageComponent,
        ActionButtonComponent,
        IllustrationComponent
      ],
      providers: [TitleService],
      imports: [TranslateModule.forRoot(), RouterModule.forRoot([])]
    }).compileComponents();
  });

  it('should display loading indicator', () => {
    const fixture = TestBed.createComponent(IsLoadingTrueTestComponent);
    fixture.detectChanges();

    const loadingIndicatorElements = fixture.debugElement.queryAll(By.directive(LoadingIndicatorComponent));
    expect(loadingIndicatorElements).toHaveSize(1);
    expect(loadingIndicatorElements[0]).toBeTruthy();

    const contentElements = fixture.debugElement.queryAll(By.css('p'));
    expect(contentElements).toHaveSize(0);
  });

  it('should display content', () => {
    const fixture = TestBed.createComponent(IsLoadingFalseTestComponent);
    fixture.detectChanges();

    const loadingIndicatorElements = fixture.debugElement.queryAll(By.directive(LoadingIndicatorComponent));
    expect(loadingIndicatorElements).toHaveSize(0);

    const contentElements = fixture.debugElement.queryAll(By.css('p'));
    expect(contentElements).toHaveSize(1);
    expect((<HTMLElement>contentElements[0].nativeElement).textContent).toBe('Hello World');
  });

  it('should display 404 error', () => {
    const fixture = TestBed.createComponent(IsLoadingFalseWith404ErrorTestComponent);
    fixture.detectChanges();

    const loadingErrorElements = fixture.debugElement.queryAll(By.directive(LoadingErrorComponent));
    expect(loadingErrorElements).toHaveSize(1);
    expect(loadingErrorElements[0]).toBeTruthy();

    let errorPageDivElements = loadingErrorElements[0].queryAll(By.css('div#resourceNotFoundError'));
    expect(errorPageDivElements).toHaveSize(1);

    errorPageDivElements = loadingErrorElements[0].queryAll(By.css('div#unexpectedError'));
    expect(errorPageDivElements).toHaveSize(0);
  });

  it('should display non-404 error', () => {
    const fixture = TestBed.createComponent(IsLoadingFalseWith500ErrorTestComponent);
    fixture.detectChanges();

    const loadingErrorElements = fixture.debugElement.queryAll(By.directive(LoadingErrorComponent));
    expect(loadingErrorElements).toHaveSize(1);
    expect(loadingErrorElements[0]).toBeTruthy();

    let errorPageDivElements = loadingErrorElements[0].queryAll(By.css('div#resourceNotFoundError'));
    expect(errorPageDivElements).toHaveSize(0);

    errorPageDivElements = loadingErrorElements[0].queryAll(By.css('div#unexpectedError'));
    expect(errorPageDivElements).toHaveSize(1);
  });
});
