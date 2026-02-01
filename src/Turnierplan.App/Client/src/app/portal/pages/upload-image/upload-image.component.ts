import { Component, OnDestroy } from '@angular/core';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { OrganizationDto } from '../../../api/models/organization-dto';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { filter } from 'rxjs/operators';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';

@Component({
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslatePipe,
    TranslateDirective,
    ReactiveFormsModule,
    TooltipIconComponent,
    AsyncPipe,
    ActionButtonComponent
  ],
  templateUrl: './upload-image.component.html'
})
export class UploadImageComponent implements OnDestroy {
  protected loadingState: LoadingState = { isLoading: false };

  protected organization?: OrganizationDto;
  protected imageName = new FormControl('', { nonNullable: true });
  protected selectedFile?: File;
  protected fallbackName?: string;
  protected previewUrl$: Observable<string>;

  private arrayBuffer$: BehaviorSubject<Blob | undefined>;

  constructor() {
    this.arrayBuffer$ = new BehaviorSubject<Blob | undefined>(undefined);
    this.previewUrl$ = this.arrayBuffer$.pipe(
      filter((blob) => !!blob),
      map((blob) => URL.createObjectURL(blob))
    );
  }

  public ngOnDestroy() {
    this.arrayBuffer$.complete();
  }

  protected onFileSelected(event: unknown): void {
    const file = (event as { target: HTMLInputElement }).target.files?.item(0);

    if (!file) {
      return;
    }

    this.selectedFile = file;
    this.setFallbackFileName(file.name);

    this.selectedFile.arrayBuffer().then((arrayBuffer): void => {
      this.arrayBuffer$.next(new Blob([arrayBuffer]));
    });
  }

  protected confirmButtonClicked(): void {
    alert('Not Implemented'); // TODO
  }

  private setFallbackFileName(name: string): void {
    // Cut off the file extension
    const i = name.lastIndexOf('.');

    if (i === -1) {
      this.fallbackName = name.trim();
    } else {
      const extensions = name.substring(i);
      if (extensions === '.png' || extensions === '.jpg' || extensions === '.jpg') {
        this.fallbackName = name.substring(0, i).trim();
      } else {
        this.fallbackName = name.trim();
      }
    }

    if (this.fallbackName.length === 0) {
      // Don't bother with translating this string because realistically, this will never happen
      this.fallbackName = 'Unnamed Image';
    }
  }
}
