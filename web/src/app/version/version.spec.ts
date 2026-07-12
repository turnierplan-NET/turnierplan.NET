import { TestBed } from '@angular/core/testing';

import { Version } from './version';

describe('Version', () => {
  let service: Version;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Version);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
