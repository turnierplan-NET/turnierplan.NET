import { Pipe, PipeTransform } from '@angular/core';
import { formatNumber } from '@angular/common';

@Pipe({
  name: 'fileSize'
})
export class FileSizePipe implements PipeTransform {
  public transform(value: number, locale: string): string {
    let suffix = '';
    let digitsFormat = '1.1-1';

    if (value >= 1_000_000_000) {
      value /= 1_000_000_000;
      suffix = 'GB';
    } else if (value >= 1_000_000) {
      value /= 1_000_000;
      suffix = 'MB';
    } else if (value >= 1_000) {
      value /= 1_000;
      suffix = 'kB';
    } else {
      suffix = 'B';
      digitsFormat = '1.0';
    }

    return formatNumber(value, locale, digitsFormat) + ' ' + suffix;
  }
}
