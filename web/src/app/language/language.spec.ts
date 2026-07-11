import { TestBed } from '@angular/core/testing';

import { Language } from './language';

describe('Language', () => {
  let service: Language;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Language);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
