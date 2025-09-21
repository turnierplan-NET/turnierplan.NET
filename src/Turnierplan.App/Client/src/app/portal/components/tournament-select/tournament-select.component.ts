import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { FolderTreeComponent, FolderTreeEntry } from '../folder-tree/folder-tree.component';
import { TranslateDirective } from '@ngx-translate/core';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getTournaments } from '../../../api/fn/tournaments/get-tournaments';

@Component({
  templateUrl: './tournament-select.component.html',
  styleUrl: './tournament-select.component.scss',
  imports: [LoadingStateDirective, TranslateDirective, FolderTreeComponent]
})
export class TournamentSelectComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: true };
  protected folderTree: FolderTreeEntry[] = [];
  protected selectedNode?: FolderTreeEntry;

  private organizationId!: string;
  private organizationName!: string;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly turnierplanApi: TurnierplanApi
  ) {}

  public set organization(value: { name: string; id: string }) {
    this.organizationId = value.id;
    this.organizationName = value.name;
  }

  public ngOnInit(): void {
    this.turnierplanApi.invoke(getTournaments, { organizationId: this.organizationId }).subscribe({
      next: (tournaments) => {
        this.folderTree = FolderTreeComponent.generateTree(this.organizationName, tournaments);
        this.selectTreeEntry('/');
        this.loadingState = { isLoading: false };
      },
      error: (error) => {
        this.modal.dismiss({ isApiError: true, apiError: error as unknown });
      }
    });
  }

  protected selectTreeEntry(id: string): void {
    this.selectedNode = this.folderTree.find((x) => x.id === id);
  }
}
