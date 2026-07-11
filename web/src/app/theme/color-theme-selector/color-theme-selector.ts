import { Component, inject } from '@angular/core';
import { ColorTheme, ColorThemes } from '../color-theme';

@Component({
  selector: 'tp-color-theme-selector',
  templateUrl: './color-theme-selector.html',
})
export class ColorThemeSelector {
  protected readonly colorTheme = inject(ColorTheme);
  protected readonly colorThemes = ColorThemes;
}
