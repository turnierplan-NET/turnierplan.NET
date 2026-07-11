import { TestBed } from '@angular/core/testing';

import { Language } from './language';
import { provideTranslateService } from '@ngx-translate/core';

describe('Language', () => {
  let service: Language;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideTranslateService()]
    });
    service = TestBed.inject(Language);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
