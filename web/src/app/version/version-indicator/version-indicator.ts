import { Component, signal } from '@angular/core';
import { appVersion } from '../../app.config';

type VersionIndicatorState = 'loading' | 'error' | 'up-to-date' | 'outdated';

@Component({
  selector: 'tp-version-indicator',
  imports: [],
  templateUrl: './version-indicator.html',
  styleUrl: './version-indicator.scss'
})
export class VersionIndicator {
  protected readonly version = signal(appVersion);
  protected readonly indicator = signal<VersionIndicatorState>('up-to-date');

  // TODO: Load & display release information from GitHub
}
