import { Component, Input } from '@angular/core';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'tp-share-link',
  standalone: false,
  templateUrl: './share-link.component.html'
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
