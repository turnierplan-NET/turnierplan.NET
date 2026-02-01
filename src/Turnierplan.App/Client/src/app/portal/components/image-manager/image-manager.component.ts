import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { ImageDto } from '../../../api/models/image-dto';
import { TranslateDirective } from '@ngx-translate/core';
import { AsyncPipe, DecimalPipe } from '@angular/common';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { setImageName } from '../../../api/fn/images/set-image-name';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { RbacWidgetComponent } from '../rbac-widget/rbac-widget.component';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { deleteImage } from '../../../api/fn/images/delete-image';
import { NotificationService } from '../../../core/services/notification.service';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { FileSizePipe } from '../../pipes/file-size.pipe';

type ImageView = ImageDto & { referenceCount?: number; isUpdatingName: boolean };

@Component({
  selector: 'tp-image-manager',
  imports: [
    TranslateDirective,
    AsyncPipe,
    RenameButtonComponent,
    DecimalPipe,
    SmallSpinnerComponent,
    IsActionAllowedDirective,
    RbacWidgetComponent,
    DeleteButtonComponent,
    TooltipIconComponent,
    TranslateDatePipe,
    FileSizePipe
  ],
  templateUrl: './image-manager.component.html'
})
export class ImageManagerComponent {
  @Input({ required: true })
  public set images(value: { images: ImageDto[]; references?: { [key: string]: number } }) {
    this._images = value.images.map((x) => ({
      ...x,
      referenceCount: value.references ? value.references[x.id] : undefined,
      isUpdatingName: false
    }));
  }

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly authorizationService = inject(AuthorizationService);
  protected readonly Actions = Actions;

  protected _images: ImageView[] = [];

  private readonly turnierplanApi = inject(TurnierplanApi);
  private readonly notificationService = inject(NotificationService);

  protected renameImage(id: string, name: string): void {
    if (this._images.some((x) => x.isUpdatingName)) {
      return;
    }

    const image = this._images.find((x) => x.id === id);

    if (!image) {
      return;
    }

    image.isUpdatingName = true;

    this.turnierplanApi.invoke(setImageName, { id: id, body: { name: name } }).subscribe({
      next: () => {
        image.isUpdatingName = false;
        image.name = name;
      },
      error: (error) => {
        this.errorOccured.emit(error);
      }
    });
  }

  protected deleteImage(id: string): void {
    this.turnierplanApi.invoke(deleteImage, { id: id }).subscribe({
      next: () => {
        this._images = this._images.filter((x) => x.id !== id);

        this.notificationService.showNotification(
          'info',
          'Portal.ViewOrganization.Images.DeleteToast.Title',
          'Portal.ViewOrganization.Images.DeleteToast.Message'
        );
      },
      error: (error) => {
        this.errorOccured.emit(error);
      }
    });
  }
}
