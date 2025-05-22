import { Component, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';

import { environment } from '../../../../environments/environment';

@Component({
  selector: 'tp-share-widget',
  templateUrl: './share-widget.component.html'
})
export class ShareWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public showQrCode: boolean = true;

  @Input()
  public viewsCounter?: number;

  protected resourceUrl: string = '';
  protected downloadName?: string;
  protected qrCodeUrl?: SafeUrl;

  @Input()
  public set resourcePath(value: string) {
    this.resourceUrl = `${environment.originOverwrite ?? window.location.origin}${value}`;
  }

  @Input()
  public set resourceName(value: string) {
    const sanitizer = /[^A-Za-z0-9 _.+-]/;
    this.downloadName = `${value.replace(sanitizer, '_')}.png`;
  }
}
