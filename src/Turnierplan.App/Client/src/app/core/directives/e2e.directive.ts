import { Directive, ElementRef, Input } from '@angular/core';
import { environment } from '../../../environments/environment';

@Directive({
  selector: '[tpE2E]'
})
export class E2eDirective {
  constructor(private readonly elementRef: ElementRef<HTMLElement>) {}

  @Input()
  public set tpE2E(value: string | (string | number)[]) {
    if (!environment.includeE2EData) {
      return;
    }

    this.elementRef.nativeElement.dataset.testid = E2eDirective.transformValue(value);
  }

  private static transformValue(value: string | (string | number)[]): string {
    if (typeof value === 'string') {
      return value;
    }

    let result = '';

    for (let i = 0; i < value.length; i++) {
      result += value[i];
      if (i < value.length - 1) {
        result += '-';
      }
    }

    return result;
  }
}
