import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VersionIndicator } from './version/version-indicator/version-indicator';
import { ColorThemeSelector } from './theme/color-theme-selector/color-theme-selector';

@Component({
  selector: 'tp-root',
  imports: [RouterLink, VersionIndicator, ColorThemeSelector],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
