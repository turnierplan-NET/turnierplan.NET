import { FileSizePipe } from './file-size.pipe';

describe('FileSizePipe', () => {
  it('formats values as expected', () => {
    const pipe = new FileSizePipe();

    expect(pipe.transform(189, 'de')).toBe('189 B');
    expect(pipe.transform(999, 'de')).toBe('999 B');
    expect(pipe.transform(1000, 'de')).toBe('1,0 kB');
    expect(pipe.transform(1495, 'de')).toBe('1,5 kB');
    expect(pipe.transform(1999, 'de')).toBe('2,0 kB');
    expect(pipe.transform(95_456, 'de')).toBe('95,5 kB');
    expect(pipe.transform(822_300, 'de')).toBe('822,3 kB');
    expect(pipe.transform(1_456_488, 'de')).toBe('1,5 MB');
    expect(pipe.transform(141_300_000, 'de')).toBe('141,3 MB');
    expect(pipe.transform(1_000_000_000, 'de')).toBe('1,0 GB');
    expect(pipe.transform(195_000_000_000, 'de')).toBe('195,0 GB');
    expect(pipe.transform(1_000_000_000_000, 'de')).toBe('1.000,0 GB');
  });
});
