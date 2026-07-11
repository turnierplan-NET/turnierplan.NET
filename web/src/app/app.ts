import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { VersionIndicator } from './version/version-indicator/version-indicator';

@Component({
  selector: 'tp-root',
  imports: [RouterLink, VersionIndicator],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
