import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VersionIndicator } from './version-indicator';
import { provideTranslateService } from '@ngx-translate/core';

describe('VersionIndicator', () => {
  let component: VersionIndicator;
  let fixture: ComponentFixture<VersionIndicator>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VersionIndicator],
      providers: [provideTranslateService()]
    }).compileComponents();

    fixture = TestBed.createComponent(VersionIndicator);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
