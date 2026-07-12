import { Component, inject } from '@angular/core';
import { Version } from '../version';

@Component({
  selector: 'tp-version-indicator',
  imports: [],
  templateUrl: './version-indicator.html',
  styleUrl: './version-indicator.scss'
})
export class VersionIndicator {
  protected readonly versionInfo = inject(Version).version;
}
