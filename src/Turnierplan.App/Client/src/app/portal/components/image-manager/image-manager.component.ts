import { Component, inject, Input } from '@angular/core';
import { ImageDto } from '../../../api/models/image-dto';
import { TranslateDirective } from '@ngx-translate/core';
import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { CopyToClipboardComponent } from '../copy-to-clipboard/copy-to-clipboard.component';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';

@Component({
  selector: 'tp-image-manager',
  imports: [TranslateDirective, AsyncPipe, RenameButtonComponent, CopyToClipboardComponent, DecimalPipe, TooltipIconComponent],
  templateUrl: './image-manager.component.html'
})
export class ImageManagerComponent {
  @Input({ required: true })
  public images: ImageDto[] = [];

  protected readonly authorizationService = inject(AuthorizationService);
  protected readonly Actions = Actions;

  protected renameImage(id: string, name: string): void {
    alert(name);
    // TODO: Implement renaming the image
  }
}
