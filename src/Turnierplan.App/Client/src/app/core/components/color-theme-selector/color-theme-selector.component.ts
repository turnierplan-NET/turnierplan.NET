import { Component, inject, Input } from '@angular/core';
import { ColorTheme, ColorThemeService } from '../../services/color-theme.service';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Visibility } from '../../../api/models/visibility';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'tp-color-theme-selector',
  imports: [TranslateDirective, TranslatePipe, NgbTooltip],
  templateUrl: './color-theme-selector.component.html'
})
export class ColorThemeSelectorComponent {
  protected readonly Visibility = Visibility;

  protected currentTheme: ColorTheme = 'light';
  protected buttonType: string = '';

  private readonly colorThemeService = inject(ColorThemeService);

  @Input()
  public mode: 'icon' | 'default' = 'default';

  constructor() {
    this.colorThemeService.theme$.pipe(takeUntilDestroyed()).subscribe((theme) => {
      this.currentTheme = theme;
      this.buttonType = theme === 'light' ? 'outline-dark' : 'outline-light';
    });
  }

  protected setTheme(theme: ColorTheme) {
    this.colorThemeService.setTheme(theme);
  }
}
