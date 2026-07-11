import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { provideRouter } from '@angular/router';
import {
  provideTranslateLoader,
  provideTranslateService,
  TranslateLoader,
  TranslationObject
} from '@ngx-translate/core';
import { Observable, of } from 'rxjs';

// IDEA: Place this class along with provideTranslateService([...]) in a central location - shared among all unit tests
class MockTranslateLoader extends TranslateLoader {
  public override getTranslation(_: string): Observable<TranslationObject> {
    return of({
      ApplicationName: 'turnierplan.NET'
    });
  }
}

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]),
        provideTranslateService({
          lang: 'en',
          loader: provideTranslateLoader(() => new MockTranslateLoader())
        })
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render application name in sidebar', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.tp-sidebar-branding')?.textContent).toContain(
      'turnierplan.NET'
    );
  });
});
