import { Component, Input } from '@angular/core';

@Component({
  selector: 'tp-share-link',
  standalone: false,
  templateUrl: './share-link.component.html'
})
export class ShareLinkComponent {
  @Input()
  public resourceUrl!: string;
}
