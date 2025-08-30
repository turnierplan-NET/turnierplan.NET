import { Component, EventEmitter, Input, Output } from '@angular/core';

import { TournamentHeaderDto } from '../../../api';
import { NgClass, NgStyle } from '@angular/common';

export type FolderTreeEntry = {
  id: string;
  folderId?: string;
  folderName?: string;
  label: string;
  isRoot: boolean;
  indentation: number;
  tournaments: TournamentHeaderDto[];
};

@Component({
  selector: 'tp-folder-tree',
  templateUrl: './folder-tree.component.html',
  imports: [NgClass, NgStyle]
})
export class FolderTreeComponent {
  @Input()
  public treeData: FolderTreeEntry[] = [];

  @Input()
  public selectedFolder: string = '/';

  @Output()
  public selectedFolderChange = new EventEmitter<string>();

  public static generateTree(organizationName: string, tournaments: TournamentHeaderDto[]): FolderTreeEntry[] {
    const treeData = [];

    const root: FolderTreeEntry = {
      id: '/',
      label: organizationName,
      isRoot: true,
      indentation: 0,
      tournaments: []
    };

    treeData.push(root);

    // Add all tournaments w/o folder to the root node
    tournaments.filter((x) => x.folderId === undefined).forEach((tournament) => root.tournaments.push(tournament));

    // Get all distinct folders
    const allFolders = FolderTreeComponent.detectFolders(tournaments);
    allFolders.sort((a, b) => a.name.localeCompare(b.name));

    // Add all tournaments with folder to the corresponding folder node
    allFolders.forEach((folder) => {
      const folderTournaments = tournaments.filter((tournament) => tournament.folderId === folder.id);
      folderTournaments.sort((a, b) => a.name.localeCompare(b.name));

      const folderEntry: FolderTreeEntry = {
        id: '/' + folder.id,
        folderId: folder.id,
        folderName: folder.name,
        label: folder.name ?? '?',
        isRoot: false,
        indentation: 1,
        tournaments: folderTournaments
      };

      treeData.push(folderEntry);
    });

    return treeData;
  }

  private static detectFolders(tournaments: TournamentHeaderDto[]): { id: string; name: string }[] {
    const result: { id: string; name: string }[] = [];

    for (const tournament of tournaments) {
      if (tournament.folderId && !result.some((x) => x.id === tournament.folderId)) {
        const folderName = tournament?.folderName;
        result.push({ id: tournament.folderId, name: folderName ?? '?' });
      }
    }

    return result;
  }
}
