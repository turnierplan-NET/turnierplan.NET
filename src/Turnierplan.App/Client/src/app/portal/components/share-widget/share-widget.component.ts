import { Component, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { TranslateDirective, TranslateService } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ShareLinkComponent } from '../share-link/share-link.component';
import { QRCodeComponent } from 'angularx-qrcode';
import { AutoReloadToggleComponent } from '../auto-reload-toggle/auto-reload-toggle.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LocalStorageService } from '../../services/local-storage.service';

@Component({
  selector: 'tp-share-widget',
  templateUrl: './share-widget.component.html',
  imports: [TranslateDirective, NgClass, ShareLinkComponent, QRCodeComponent, AutoReloadToggleComponent, ReactiveFormsModule, FormsModule]
})
export class ShareWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public viewsCounter?: number;

  @Input()
  public resourcePath: string = '';

  @Input()
  public isForFullscreenView = false;

  @Input()
  public autoReloadMinInterval: number = 3;

  protected downloadName?: string;
  protected qrCodeUrl?: SafeUrl;
  protected autoReloadPathSuffix = '';
  protected includeQrCodeOnFullscreenView: boolean;

  constructor(private readonly localStorageService: LocalStorageService) {
    this.includeQrCodeOnFullscreenView = localStorageService.getIncludeQrCodeOnFullscreenView();
  }

  @Input()
  public set resourceName(value: string) {
    const sanitizer = /[^A-Za-z0-9 _.+-]/;
    this.downloadName = `${value.replace(sanitizer, '_')}.png`;
  }

  protected get showQrCode(): boolean {
    return !this.isForFullscreenView;
  }
}
