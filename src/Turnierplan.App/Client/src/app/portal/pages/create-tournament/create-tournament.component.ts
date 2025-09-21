import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { combineLatest, from, of, Subject, switchMap, takeUntil } from 'rxjs';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { LocalStorageService } from '../../services/local-storage.service';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { VisibilitySelectorComponent } from '../../components/visibility-selector/visibility-selector.component';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';

type FolderMode = 'NoFolder' | 'ExistingFolder' | 'NewFolder';

@Component({
  templateUrl: './create-tournament.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslateDirective,
    FormsModule,
    ReactiveFormsModule,
    NgClass,
    TooltipIconComponent,
    VisibilitySelectorComponent,
    ActionButtonComponent,
    TranslatePipe
  ]
})
export class CreateTournamentComponent implements OnDestroy {
  protected loadingState: LoadingState = { isLoading: false };

  protected organization?: OrganizationDto;
  protected existingFolders?: FolderDto[];
  protected tournamentName = new FormControl('', { nonNullable: true });
  protected folderMode: FolderMode = 'NoFolder';
  protected existingFolderId = new FormControl<string | undefined>('');
  protected newFolderName = new FormControl<string | undefined>('');
  protected visibility: Visibility = Visibility.Public;

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly localStorageService: LocalStorageService,
    private readonly titleService: TitleService
  ) {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const organizationId = params.get('id');
          if (organizationId === null) {
            this.loadingState = { isLoading: false };
            return of([undefined, undefined]);
          }
          this.loadingState = { isLoading: true };
          return combineLatest([
            this.organizationService.getOrganization({ id: organizationId }),
            this.folderService.getFolders({ organizationId: organizationId })
          ]);
        })
      )
      .subscribe({
        next: ([organization, folders]) => {
          this.organization = organization;
          this.existingFolders = folders;

          this.titleService.setTitleFrom(organization);

          this.folderMode = 'NoFolder';
          this.newFolderName.setValue('');

          if (this.existingFolders && this.existingFolders.length > 0) {
            this.existingFolders.sort((a, b) => a.name.localeCompare(b.name));
            this.existingFolderId.setValue(this.existingFolders[0].id);
          } else {
            this.existingFolderId.setValue('');
          }

          if (organization && folders) {
            const currentFolder = this.localStorageService.getCurrentFolderId(organization.id);
            if (currentFolder) {
              const index = currentFolder.indexOf('/');
              if (index >= 0) {
                const folderId = currentFolder.substring(index + 1);
                if (folders.findIndex((x) => x.id === folderId) >= 0) {
                  this.folderMode = 'ExistingFolder';
                  this.existingFolderId.setValue(currentFolder.substring(index + 1));
                }
              }
            }
          }

          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected get disableExistingFolders(): boolean {
    return !this.existingFolders || this.existingFolders.length === 0;
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  protected confirmButtonClicked(): void {
    if (this.loadingState.isLoading || !this.organization) {
      return;
    }

    if (!this.tournamentName.valid) {
      return;
    }

    if (this.folderMode === 'ExistingFolder' && !this.existingFolderId.valid) {
      return;
    }

    if (this.folderMode === 'NewFolder' && !this.newFolderName.valid) {
      return;
    }

    if (this.tournamentName.valid && !this.loadingState.isLoading && this.organization) {
      this.loadingState = { isLoading: true };

      this.tournamentService
        .createTournament({
          body: {
            organizationId: this.organization.id,
            name: this.tournamentName.value,
            folderId: this.folderMode === 'ExistingFolder' ? (this.existingFolderId.value ?? undefined) : undefined,
            folderName: this.folderMode === 'NewFolder' ? (this.newFolderName.value ?? undefined) : undefined,
            visibility: this.visibility
          }
        })
        .pipe(
          // Navigate to the configuration page because that is usually where you would go after creating a tournament
          switchMap((tournament) =>
            from(this.router.navigate(['../../../../tournament', tournament.id, 'configure'], { relativeTo: this.route }))
          )
        )
        .subscribe({
          next: () => {
            this.loadingState = { isLoading: false };
          },
          error: (error) => {
            this.loadingState = { isLoading: false, error: error };
          }
        });
    }
  }
}
