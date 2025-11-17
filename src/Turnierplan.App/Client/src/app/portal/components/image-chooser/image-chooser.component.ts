import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Actions } from '../../../generated/actions';
import { TranslateDirective } from '@ngx-translate/core';
import { LoadingIndicatorComponent } from '../loading-indicator/loading-indicator.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { AlertComponent } from '../alert/alert.component';
import { NgClass } from '@angular/common';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { ImageType } from '../../../api/models/image-type';
import { uploadImage$FormData } from '../../../api/fn/images/upload-image-form-data';
import { getImages } from '../../../api/fn/images/get-images';
import { ImageDto } from '../../../api/models/image-dto';
import { deleteImage } from '../../../api/fn/images/delete-image';

export interface ImageChooserResult {
  type: 'ImageDeleted' | 'ImageSelected' | 'ImageUploaded';
  image?: ImageDto;
  deletedImageId?: string;
}

@Component({
  templateUrl: './image-chooser.component.html',
  styleUrls: ['./image-chooser.component.scss'],
  imports: [
    TranslateDirective,
    LoadingIndicatorComponent,
    IsActionAllowedDirective,
    DeleteButtonComponent,
    SmallSpinnerComponent,
    ActionButtonComponent,
    AlertComponent,
    NgClass,
    TranslateDatePipe
  ]
})
export class ImageChooserComponent {
  protected readonly Actions = Actions;

  protected isInitialized = false;

  protected organizationId!: string;
  protected imageType!: ImageType;

  protected existingImages: ImageDto[] = [];
  protected isLoadingImages = true;
  protected currentImageId?: string;

  protected isUploadingImage = false;
  protected hasUploadError = false;

  // On mobile, where hovering does not work, the current "hovered" image is set by clicking and stored in this variable.
  protected hoverOverrideImageId?: string;

  protected imageForDetailView?: ImageDto;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly turnierplanApi: TurnierplanApi
  ) {}

  protected get isInImageDetailView(): boolean {
    return this.imageForDetailView !== undefined;
  }

  public init(organizationId: string, imageType: ImageType, currentImageId?: string): void {
    if (this.isInitialized) {
      return;
    }

    this.organizationId = organizationId;
    this.imageType = imageType;
    this.currentImageId = currentImageId;
    this.isInitialized = true;

    this.loadImages();
  }

  protected openFileSelectionDialog(): void {
    if (this.isUploadingImage) {
      return;
    }

    const tempElement = document.createElement('input');
    tempElement.type = 'file';
    tempElement.accept = 'image/png,image/jpeg';

    tempElement.addEventListener('change', (event) => {
      const targetFile = (event.target as HTMLInputElement).files?.item(0);

      if (targetFile) {
        this.isUploadingImage = true;

        const reader = new FileReader();

        reader.onload = (data) => {
          const content = data.target?.result as ArrayBuffer;

          if (content) {
            this.turnierplanApi
              .invoke(uploadImage$FormData, {
                body: {
                  organizationId: this.organizationId,
                  imageType: this.imageType,
                  imageName: targetFile.name,
                  image: new Blob([content])
                }
              })
              .subscribe({
                next: (result) => {
                  this.modal.close({ type: 'ImageUploaded', image: result } as ImageChooserResult);
                },
                error: () => {
                  this.isUploadingImage = false;
                  this.hasUploadError = true;
                }
              });
          }
        };

        reader.readAsArrayBuffer(targetFile);
      }
    });

    tempElement.click();
  }

  protected getRoundedFileSize(fileSize: number): number {
    return Math.round(fileSize / 1000);
  }

  protected deleteCurrentViewedImage(): void {
    if (!this.imageForDetailView) {
      return;
    }

    const deleteImageId = this.imageForDetailView.id;
    this.imageForDetailView = undefined;

    this.isLoadingImages = true;

    this.turnierplanApi.invoke(deleteImage, { id: deleteImageId }).subscribe({
      next: () => {
        if (deleteImageId === this.currentImageId) {
          this.modal.close({ type: 'ImageDeleted', deletedImageId: deleteImageId } as ImageChooserResult);
        } else {
          this.loadImages();
        }
      },
      error: (error) => {
        this.modal.dismiss({ isApiError: true, apiError: error as unknown });
      }
    });
  }

  private loadImages(): void {
    this.turnierplanApi.invoke(getImages, { organizationId: this.organizationId, imageType: this.imageType }).subscribe({
      next: (images) => {
        this.existingImages = images;
        this.existingImages.sort((a, b) => {
          if (a.id === this.currentImageId) return -1;
          if (b.id === this.currentImageId) return 1;
          return new Date(b.createdAt).getDate() - new Date(a.createdAt).getDate();
        });
        this.isLoadingImages = false;
      },
      error: (error) => {
        this.modal.dismiss({ isApiError: true, apiError: error as unknown });
      }
    });
  }
}
