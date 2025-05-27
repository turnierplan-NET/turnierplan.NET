import { Component, EventEmitter, Injector, Input, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { ImageDto, ImageType } from '../../../api';
import { ImageChooserComponent } from '../image-chooser/image-chooser.component';

@Component({
  standalone: false,
  selector: 'tp-image-widget',
  templateUrl: './image-widget.component.html'
})
export class ImageWidgetComponent {
  @Input()
  public currentImage?: ImageDto;

  @Input()
  public imageAlt!: string;

  @Input()
  public imageType!: ImageType;

  @Input()
  public organizationId!: string;

  @Output()
  public imageChange = new EventEmitter<string>();

  @Output()
  public apiError = new EventEmitter<unknown>();

  constructor(
    private readonly modalService: NgbModal,
    private readonly injector: Injector
  ) {}

  protected openSelectionDialog(): void {
    const ref = this.modalService.open(ImageChooserComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg',
      injector: this.injector
    });

    const component = ref.componentInstance as ImageChooserComponent;
    component.init(this.organizationId, this.imageType, this.currentImage?.id);

    ref.closed.subscribe({
      next: (value: string) => {
        this.imageChange.emit(value);
      }
    });

    ref.dismissed.subscribe({
      next: (reason?: { isApiError?: boolean; apiError?: unknown }) => {
        if (reason?.isApiError === true) {
          // If reason is specified, this means an error occurred
          this.apiError.emit(reason.apiError);
        }
      }
    });
  }
}
