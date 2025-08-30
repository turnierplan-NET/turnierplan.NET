import { Component, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { TranslateDirective } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ShareLinkComponent } from '../share-link/share-link.component';
import { QRCodeComponent } from 'angularx-qrcode';

@Component({
    selector: 'tp-share-widget',
    templateUrl: './share-widget.component.html',
    imports: [TranslateDirective, NgClass, ShareLinkComponent, QRCodeComponent]
})
export class ShareWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public showQrCode: boolean = true;

  @Input()
  public viewsCounter?: number;

  @Input()
  public resourcePath: string = '';

  protected downloadName?: string;
  protected qrCodeUrl?: SafeUrl;

  @Input()
  public set resourceName(value: string) {
    const sanitizer = /[^A-Za-z0-9 _.+-]/;
    this.downloadName = `${value.replace(sanitizer, '_')}.png`;
  }
}
