import { Component, inject } from '@angular/core';
import { Version } from '../version';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-version-indicator',
  imports: [NgbTooltip, TranslatePipe],
  templateUrl: './version-indicator.html',
  styleUrl: './version-indicator.scss'
})
export class VersionIndicator {
  protected readonly versionInfo = inject(Version).version;
}
