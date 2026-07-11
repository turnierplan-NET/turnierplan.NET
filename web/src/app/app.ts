import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VersionIndicator } from './version/version-indicator/version-indicator';
import { ColorThemeSelector } from './theme/color-theme-selector/color-theme-selector';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageSelector } from './language/language-selector/language-selector';
import { Language } from './language/language';

@Component({
  selector: 'tp-root',
  imports: [RouterLink, VersionIndicator, ColorThemeSelector, TranslatePipe, LanguageSelector],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly language = inject(Language);
}
