import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'tp-copy-to-clipboard',
  templateUrl: './copy-to-clipboard.component.html'
})
export class CopyToClipboardComponent {
  @Input()
  public value: string = '';

  protected copyToClipboardPressed = false;

  protected copyToClipboard(): void {
    if (this.copyToClipboardPressed) {
      return;
    }

    void navigator.clipboard.writeText(this.value).then(() => {
      this.copyToClipboardPressed = true;
      setTimeout(() => {
        this.copyToClipboardPressed = false;
      }, 2500);
    });
  }
}
