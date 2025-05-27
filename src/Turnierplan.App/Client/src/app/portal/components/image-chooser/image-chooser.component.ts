import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { ImageDto2, ImageType, ImagesService } from '../../../api';

@Component({
  templateUrl: './image-chooser.component.html',
  styleUrls: ['./image-chooser.component.scss']
})
export class ImageChooserComponent {
  protected isInitialized = false;

  protected organizationId!: string;
  protected imageType!: ImageType;

  protected existingImages: ImageDto2[] = [];
  protected isLoadingImages = true;
  protected currentImageId?: string;

  protected isUploadingImage = false;
  protected hasUploadError = false;

  // On mobile, where hovering does not work, the current "hovered" image is set by clicking and stored in this variable.
  protected hoverOverrideImageId?: string;

  protected imageForDetailView?: ImageDto2;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly imageService: ImagesService
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
            this.imageService
              .uploadImage$FormData({
                body: {
                  organizationId: this.organizationId,
                  imageType: this.imageType,
                  imageName: targetFile.name,
                  image: new Blob([content])
                }
              })
              .subscribe({
                next: (result) => {
                  this.modal.close(result.id);
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

    this.imageService.deleteImage({ id: deleteImageId }).subscribe({
      next: () => {
        if (deleteImageId === this.currentImageId) {
          // Close modal with undefined to trigger re-load of tournament page
          this.modal.close();
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
    this.imageService.getImages({ organizationId: this.organizationId, imageType: this.imageType }).subscribe({
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
