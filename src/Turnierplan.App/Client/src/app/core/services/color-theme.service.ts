import { inject, Injectable } from '@angular/core';
import { LocalStorageService } from '../../portal/services/local-storage.service';
import { map, Observable, ReplaySubject } from 'rxjs';

export type ColorTheme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ColorThemeService {
  private readonly localStorageService = inject(LocalStorageService);
  private readonly themeSubject$ = new ReplaySubject<ColorTheme>(1);

  public get theme$(): Observable<ColorTheme> {
    return this.themeSubject$.asObservable();
  }

  public get isDarkMode$(): Observable<boolean> {
    return this.themeSubject$.pipe(map((theme) => theme === 'dark'));
  }

  public initialize(): void {
    const currentTheme = this.localStorageService.getColorTheme() === 'dark' ? 'dark' : 'light';
    ColorThemeService.setColorTheme(currentTheme);
    this.themeSubject$.next(currentTheme);
  }

  public setTheme(theme: ColorTheme): void {
    this.localStorageService.setColorTheme(theme);
    ColorThemeService.setColorTheme(theme);
    this.themeSubject$.next(theme);
  }

  private static setColorTheme(theme: ColorTheme): void {
    document.documentElement.dataset.bsTheme = theme;
  }
}
