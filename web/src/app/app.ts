import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { appVersion } from './app.config';

@Component({
  selector: 'tp-root',
  templateUrl: './app.html',
  imports: [RouterLink],
  styleUrl: './app.scss',
})
export class App {
  protected readonly version = signal(appVersion);
}
