import { Component, inject, OnDestroy } from '@angular/core';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { OrganizationDto } from '../../../api/models/organization-dto';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { BehaviorSubject, map, Observable, of, switchMap } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { filter } from 'rxjs/operators';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { LoadingIndicatorComponent } from '../../components/loading-indicator/loading-indicator.component';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { getOrganization } from '../../../api/fn/organizations/get-organization';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { TitleService } from '../../services/title.service';
import { uploadImage$FormData } from '../../../api/fn/images/upload-image-form-data';
import { LocalStorageService } from '../../services/local-storage.service';
import { ViewOrganizationComponent } from '../view-organization/view-organization.component';

@Component({
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslatePipe,
    TranslateDirective,
    ReactiveFormsModule,
    TooltipIconComponent,
    AsyncPipe,
    ActionButtonComponent,
    LoadingIndicatorComponent
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
  protected uploadStarted = false;

  private readonly localStorageService = inject(LocalStorageService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly titleService = inject(TitleService);
  private readonly turnierplanApi = inject(TurnierplanApi);

  private blob$: BehaviorSubject<Blob | undefined>;

  constructor() {
    this.blob$ = new BehaviorSubject<Blob | undefined>(undefined);
    this.previewUrl$ = this.blob$.pipe(
      filter((blob) => !!blob),
      map((blob) => URL.createObjectURL(blob))
    );

    this.route.paramMap
      .pipe(
        takeUntilDestroyed(),
        switchMap((params) => {
          const organizationId = params.get('id');
          if (organizationId === null) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }

          this.loadingState = { isLoading: true };
          return this.turnierplanApi.invoke(getOrganization, { id: organizationId });
        })
      )
      .subscribe({
        next: (organization) => {
          this.organization = organization;
          this.titleService.setTitleFrom(organization);
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  public ngOnDestroy() {
    this.blob$.complete();
  }

  protected onFileSelected(event: unknown): void {
    if (this.uploadStarted) {
      return;
    }

    const file = (event as { target: HTMLInputElement }).target.files?.item(0);

    if (!file) {
      return;
    }

    this.selectedFile = file;
    this.setFallbackFileName(file.name);

    this.selectedFile.arrayBuffer().then((arrayBuffer): void => {
      this.blob$.next(new Blob([arrayBuffer]));
    });
  }

  protected confirmButtonClicked(): void {
    const currentBlob = this.blob$.value;
    const organizationId = this.organization?.id;

    if (this.uploadStarted || !currentBlob || !organizationId) {
      return;
    }

    this.uploadStarted = true;

    let imageName = this.imageName.value.trim();
    if (imageName.length === 0) {
      imageName = this.fallbackName ?? '-';
    }

    this.turnierplanApi
      .invoke(uploadImage$FormData, {
        body: {
          organizationId: organizationId,
          imageName: imageName,
          image: currentBlob
        }
      })
      .subscribe({
        next: (): void => {
          this.localStorageService.setNavigationTab(organizationId, ViewOrganizationComponent.imagesPageId);
          void this.router.navigate(['../../'], { relativeTo: this.route });
        },
        error: (error): void => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
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
