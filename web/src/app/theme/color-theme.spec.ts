import { TestBed } from '@angular/core/testing';

import { ColorTheme } from './color-theme';

describe('ColorTheme', () => {
  let service: ColorTheme;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ColorTheme);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
