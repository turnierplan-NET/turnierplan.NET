import { Directive, ElementRef, Input } from '@angular/core';

@Directive({
  selector: '[tpE2E]'
})
export class E2eDirective {
  constructor(private readonly elementRef: ElementRef<HTMLElement>) {}

  @Input()
  public set tpE2E(value: string | (string | number)[]) {
    this.elementRef.nativeElement.setAttribute('data-cy', E2eDirective.transformValue(value));
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
