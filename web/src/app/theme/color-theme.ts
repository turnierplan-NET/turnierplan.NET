import { Service, Signal, signal } from '@angular/core';

export enum ColorThemes {
  Light = 'light',
  Dark = 'dark',
}

@Service()
export class ColorTheme {
  private static readonly localStorageKey = 'tp_colorTheme';

  public readonly theme: Signal<ColorThemes>;
  private readonly _theme = signal<ColorThemes>(ColorThemes.Light);

  constructor() {
    this.theme = this._theme.asReadonly();

    const currenTheme = localStorage.getItem(ColorTheme.localStorageKey);
    if (currenTheme === 'dark') {
      // Updating the HTML theme upon startup is not necessary because this is done initially in the 'index.html' file.
      this._theme.set(ColorThemes.Dark);
    }
  }

  public setTheme(theme: ColorThemes): void {
    if (theme !== this._theme()) {
      const value = theme === ColorThemes.Dark ? 'dark' : 'light';
      localStorage.setItem(ColorTheme.localStorageKey, value);
      document.documentElement.dataset['bsTheme'] = value;

      this._theme.set(theme);
    }
  }
}
