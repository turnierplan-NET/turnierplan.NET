import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';

import { OrganizationDto, TournamentHeaderDto, FoldersService } from '../../../api';
import { LocalStorageService } from '../../services/local-storage.service';
import { FolderTreeComponent, FolderTreeEntry } from '../folder-tree/folder-tree.component';

@Component({
  selector: 'tp-tournament-explorer',
  templateUrl: './tournament-explorer.component.html'
})
export class TournamentExplorerComponent implements OnChanges {
  @Input()
  public organization!: OrganizationDto;

  @Input()
  public tournaments: TournamentHeaderDto[] = [];

  @Output()
  public requestError = new EventEmitter<unknown>();

  protected currentId: string = '/';
  protected currentEntry?: FolderTreeEntry;
  protected treeData: FolderTreeEntry[] = [];
  protected isUpdatingFolderName: boolean = false;

  constructor(
    private readonly folderService: FoldersService,
    private readonly localStorageService: LocalStorageService
  ) {}

  public ngOnChanges(): void {
    this.treeData = FolderTreeComponent.generateTree(this.organization.name, this.tournaments);

    const previousId = this.localStorageService.getCurrentFolderId(this.organization.id);

    if (previousId && this.treeData.some((x) => x.id === previousId)) {
      this.toggleTreeNode(previousId);
    } else {
      this.toggleTreeNode('/');
    }
  }

  protected toggleTreeNode(id: string): void {
    if (this.isUpdatingFolderName) {
      return;
    }

    this.currentId = id;
    this.currentEntry = this.treeData.find((x) => x.id === id);
    this.localStorageService.setCurrentFolder(this.organization.id, id);
  }

  protected renameFolder(folderId: string, name: string): void {
    this.isUpdatingFolderName = true;
    this.folderService.setFolderName({ id: folderId, body: { name: name } }).subscribe({
      next: () => {
        const folder = this.treeData.find((x) => x.folderId === folderId);
        if (folder) {
          folder.label = name;
        }
        this.isUpdatingFolderName = false;
      },
      error: (error) => this.requestError.emit(error)
    });
  }
}
