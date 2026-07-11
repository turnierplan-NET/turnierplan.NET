import { inject, Service, signal, Signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { catchError, of, take } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';

export type LanguageCode = 'de' | 'en';

@Service()
export class Language {
  private static readonly localStorageKey = 'tp_language';

  public readonly language: Signal<LanguageCode>;
  public readonly isLoaded: Signal<boolean>;

  private readonly _language = signal<LanguageCode>('de');
  private readonly translateService = inject(TranslateService);

  constructor() {
    this.language = this._language.asReadonly();

    const currentLanguage = localStorage.getItem(Language.localStorageKey);
    if (currentLanguage === 'de' || currentLanguage === 'en') {
      this._language.set(currentLanguage);
    }

    this.isLoaded = toSignal(
      this.translateService.use(this._language()).pipe(
        catchError(() => of(undefined)),
        take(1),
        map(() => true)
      ),
      { initialValue: false }
    );
  }

  public setLanguage(language: LanguageCode): void {
    if (language !== this._language()) {
      localStorage.setItem(Language.localStorageKey, language);
      this.translateService.use(language);
      this._language.set(language);
    }
  }
}
