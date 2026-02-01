import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { Actions } from '../../../generated/actions';
import { TranslateDirective } from '@ngx-translate/core';
import { LoadingIndicatorComponent } from '../loading-indicator/loading-indicator.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgClass } from '@angular/common';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { ImageType } from '../../../api/models/image-type';
import { getImages } from '../../../api/fn/images/get-images';
import { ImageDto } from '../../../api/models/image-dto';

export interface ImageChooserResult {
  type: 'ImageDeleted' | 'ImageSelected';
  image?: ImageDto;
  deletedImageId?: string;
}

@Component({
  templateUrl: './image-chooser.component.html',
  styleUrls: ['./image-chooser.component.scss'],
  imports: [TranslateDirective, LoadingIndicatorComponent, ActionButtonComponent, NgClass]
})
export class ImageChooserComponent {
  protected readonly Actions = Actions;

  protected isInitialized = false;

  protected organizationId!: string;
  protected imageType!: ImageType;

  protected existingImages: ImageDto[] = [];
  protected isLoadingImages = true;
  protected currentImageId?: string;

  // On mobile, where hovering does not work, the current "hovered" image is set by clicking and stored in this variable.
  protected hoverOverrideImageId?: string;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly turnierplanApi: TurnierplanApi
  ) {}

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

  private loadImages(): void {
    this.turnierplanApi.invoke(getImages, { organizationId: this.organizationId, imageType: this.imageType }).subscribe({
      next: (response) => {
        this.existingImages = response.images;
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
