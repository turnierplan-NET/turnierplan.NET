import { Component, Input } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { CopyToClipboardComponent } from '../copy-to-clipboard/copy-to-clipboard.component';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';

@Component({
  selector: 'tp-id-widget',
  imports: [TranslateDirective, CopyToClipboardComponent, TooltipIconComponent],
  templateUrl: './id-widget.component.html'
})
export class IdWidgetComponent {
  @Input()
  public id!: string;

  @Input()
  public compact: boolean = false;
}
