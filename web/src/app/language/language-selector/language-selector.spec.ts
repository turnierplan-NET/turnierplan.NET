import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LanguageSelector } from './language-selector';
import { provideTranslateService } from '@ngx-translate/core';

describe('LanguageSelector', () => {
  let component: LanguageSelector;
  let fixture: ComponentFixture<LanguageSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LanguageSelector],
      providers: [provideTranslateService()]
    }).compileComponents();

    fixture = TestBed.createComponent(LanguageSelector);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
