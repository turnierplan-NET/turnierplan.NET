import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ColorThemeSelector } from './color-theme-selector';
import { provideTranslateService } from '@ngx-translate/core';

describe('ColorThemeSelector', () => {
  let component: ColorThemeSelector;
  let fixture: ComponentFixture<ColorThemeSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ColorThemeSelector],
      providers: [provideTranslateService()]
    }).compileComponents();

    fixture = TestBed.createComponent(ColorThemeSelector);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
