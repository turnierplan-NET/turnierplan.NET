import { Component, Input } from '@angular/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-copy-to-clipboard',
  templateUrl: './copy-to-clipboard.component.html',
  imports: [NgbTooltip, TranslatePipe]
})
export class CopyToClipboardComponent {
  private _value: string | number = '';
  private _nonce = 0;

  @Input()
  public set value(value: string | number) {
    this._value = value;
    this._nonce++;
    this.copyToClipboardPressed = false;
  }

  @Input()
  public tooltipOverwrite?: string;

  protected copyToClipboardPressed = false;

  protected copyToClipboard(): void {
    if (this.copyToClipboardPressed) {
      return;
    }

    void navigator.clipboard.writeText(`${this._value}`).then(() => {
      this.copyToClipboardPressed = true;

      const k = this._nonce;

      setTimeout(() => {
        if (k === this._nonce) {
          // Only reset 'copyToClipboardPressed' if the value was not changed in the meantime (which immediately resets the copyToClipboardPressed to false)
          this.copyToClipboardPressed = false;
        }
      }, 2500);
    });
  }
}
