import { Component, inject } from '@angular/core';
import { ColorTheme, ColorThemeService } from '../../../core/services/color-theme.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Visibility } from '../../../api/models/visibility';
import { TranslateDirective } from '@ngx-translate/core';

@Component({
  selector: 'tp-color-theme-selector',
  imports: [TranslateDirective],
  templateUrl: './color-theme-selector.component.html'
})
export class ColorThemeSelectorComponent {
  protected currentTheme: ColorTheme = 'light';
  protected buttonType: string = '';

  private readonly colorThemeService = inject(ColorThemeService);

  constructor() {
    this.colorThemeService.theme$.pipe(takeUntilDestroyed()).subscribe((theme) => {
      this.currentTheme = theme;
      this.buttonType = theme === 'light' ? 'outline-dark' : 'outline-light';
    });
  }

  protected readonly Visibility = Visibility;

  protected setTheme(theme: ColorTheme) {
    this.colorThemeService.setTheme(theme);
  }
}
