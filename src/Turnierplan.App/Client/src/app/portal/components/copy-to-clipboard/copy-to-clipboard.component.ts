import { Component, Input } from '@angular/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-copy-to-clipboard',
  templateUrl: './copy-to-clipboard.component.html',
  imports: [NgbTooltip, TranslatePipe]
})
export class CopyToClipboardComponent {
  @Input()
  public value: string | number = '';

  @Input()
  public tooltipOverwrite?: string;

  protected copyToClipboardPressed = false;

  protected copyToClipboard(): void {
    if (this.copyToClipboardPressed) {
      return;
    }

    void navigator.clipboard.writeText(`${this.value}`).then(() => {
      this.copyToClipboardPressed = true;
      setTimeout(() => {
        this.copyToClipboardPressed = false;
      }, 2500);
    });
  }
}
