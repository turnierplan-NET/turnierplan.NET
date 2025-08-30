import { Component } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

import { FolderDto, SetTournamentFolderEndpointRequest, FoldersService, NullableOfPublicId } from '../../../api';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../action-button/action-button.component';

type FolderMode = 'NoFolder' | 'ExistingFolder' | 'NewFolder';

@Component({
  templateUrl: './move-tournament-to-folder.component.html',
  imports: [
    TranslateDirective,
    FormsModule,
    TooltipIconComponent,
    SmallSpinnerComponent,
    ReactiveFormsModule,
    NgClass,
    ActionButtonComponent,
    TranslatePipe
  ]
})
export class MoveTournamentToFolderComponent {
  public save$ = new Subject<SetTournamentFolderEndpointRequest>();

  protected isSaving = false;
  protected isLoading = true;

  protected currentFolderId?: NullableOfPublicId;
  protected currentFolderName?: string;
  protected availableFolders?: FolderDto[] = undefined;

  protected folderMode: FolderMode = 'ExistingFolder';
  protected moveToFolderId: string | undefined = undefined;
  protected moveToNewFolder = new FormControl<string | undefined>('');

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly folderService: FoldersService
  ) {}

  protected get disableExistingFolders(): boolean {
    return !!this.availableFolders && this.availableFolders.length === 0;
  }

  public initialize(organizationId: string, currentFolderId: NullableOfPublicId | undefined, currentFolderName: string | undefined): void {
    this.currentFolderId = currentFolderId;
    this.currentFolderName = currentFolderName;

    this.folderService.getFolders({ organizationId: organizationId }).subscribe({
      next: (availableFolders) => {
        this.availableFolders = availableFolders.filter((x) => x.id !== currentFolderId);
        this.availableFolders.sort((a, b) => a.name.localeCompare(b.name));
        this.moveToFolderId = this.availableFolders.length === 0 ? undefined : this.availableFolders[0].id;
        if (this.folderMode === 'ExistingFolder' && this.availableFolders.length === 0) {
          this.folderMode = 'NewFolder';
        }
        this.isLoading = false;
      }
    });
  }

  protected saveClicked(): void {
    let command: SetTournamentFolderEndpointRequest | undefined = undefined;

    switch (this.folderMode) {
      case 'NoFolder':
        if (this.currentFolderId !== undefined) {
          command = {
            /* Specify nothing will reset folder */
          };
        }
        break;
      case 'ExistingFolder':
        if (this.moveToFolderId !== undefined && this.moveToFolderId !== this.currentFolderId) {
          command = { folderId: this.moveToFolderId };
        }
        break;
      case 'NewFolder':
        if (this.moveToNewFolder.invalid) {
          return;
        }
        command = { folderName: this.moveToNewFolder.value ?? undefined };
        break;
    }

    if (command !== undefined) {
      this.isSaving = true;
      this.save$.next(command);
    }

    this.save$.complete();
  }
}
