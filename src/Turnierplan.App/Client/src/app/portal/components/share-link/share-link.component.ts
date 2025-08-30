import { Component, Input } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { CopyToClipboardComponent } from '../copy-to-clipboard/copy-to-clipboard.component';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-share-link',
  templateUrl: './share-link.component.html',
  imports: [CopyToClipboardComponent, NgbTooltip, TranslatePipe]
})
export class ShareLinkComponent {
  private _resourceUrl: string = '';

  @Input()
  public set resourcePath(value: string) {
    this._resourceUrl = `${environment.originOverwrite ?? window.location.origin}${value.startsWith('/') ? '' : '/'}${value}`;
  }

  public get resourceUrl(): string {
    return this._resourceUrl;
  }
}
