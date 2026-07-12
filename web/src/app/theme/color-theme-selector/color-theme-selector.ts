import { Component, computed, inject, Signal } from '@angular/core';
import { ColorTheme, ColorThemes } from '../color-theme';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-color-theme-selector',
  imports: [NgbTooltip, TranslatePipe],
  templateUrl: './color-theme-selector.html'
})
export class ColorThemeSelector {
  protected readonly isDarkModeActive: Signal<boolean>;

  private readonly colorTheme = inject(ColorTheme);

  constructor() {
    this.isDarkModeActive = computed(() => this.colorTheme.theme() === ColorThemes.Dark);
  }

  protected switchTheme(): void {
    const current = this.colorTheme.theme();
    this.colorTheme.setTheme(current === ColorThemes.Dark ? ColorThemes.Light : ColorThemes.Dark);
  }
}
