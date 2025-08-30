import { formatDate } from '@angular/common';
import { OnDestroy, Pipe, PipeTransform } from '@angular/core';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

@Pipe({
    name: 'translateDate',
    pure: false
})
export class TranslateDatePipe implements PipeTransform, OnDestroy {
  private onLangChange?: Subscription;
  private currentValue = '';
  private lastDate?: Date | string | null;
  private lastFormat?: string;

  constructor(private readonly translateService: TranslateService) {}

  public transform(value?: Date | string | null, format = 'medium'): string {
    if (this.lastDate === value && this.lastFormat === format) {
      return this.currentValue;
    }

    this.lastDate = value;
    this.lastFormat = format;

    this.updateValue(this.translateService.currentLang ?? this.translateService.defaultLang);

    this.onLangChange ??= this.translateService.onLangChange.subscribe((event: LangChangeEvent) => {
      if (this.lastDate && this.lastFormat) {
        this.updateValue(event.lang);
      }
    });

    return this.currentValue;
  }

  public ngOnDestroy(): void {
    if (this.onLangChange) {
      this.onLangChange.unsubscribe();
      this.onLangChange = undefined;
    }
  }

  private updateValue(languageCode: string): void {
    this.currentValue = !this.lastDate || !this.lastFormat ? '' : formatDate(this.lastDate, this.lastFormat, languageCode);
  }
}
