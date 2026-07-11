import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { appVersion } from './app.config';

@Component({
  selector: 'tp-root',
  imports: [RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly version = signal(appVersion);
}
