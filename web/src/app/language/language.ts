import { inject, Service, signal, Signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { catchError, of, take } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';
import { localStorageKeys } from '../consts/local-storage-keys';

export type LanguageCode = 'de' | 'en';

@Service()
export class Language {
  public readonly language: Signal<LanguageCode>;
  public readonly isLoaded: Signal<boolean>;

  private readonly _language = signal<LanguageCode>('de');
  private readonly translateService = inject(TranslateService);

  constructor() {
    this.language = this._language.asReadonly();

    const currentLanguage = localStorage.getItem(localStorageKeys.language.currentLanguage);
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
      localStorage.setItem(localStorageKeys.language.currentLanguage, language);
      this.translateService.use(language);
      this._language.set(language);
    }
  }
}
