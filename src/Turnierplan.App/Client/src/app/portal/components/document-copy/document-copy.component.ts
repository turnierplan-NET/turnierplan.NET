import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject, switchMap, tap } from 'rxjs';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { FolderTreeComponent, FolderTreeEntry } from '../folder-tree/folder-tree.component';
import { TranslateDirective } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { DocumentDto } from '../../../api/models/document-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getTournaments } from '../../../api/fn/tournaments/get-tournaments';
import { getDocuments } from '../../../api/fn/documents/get-documents';

@Component({
  templateUrl: './document-copy.component.html',
  styleUrl: './document-copy.component.scss',
  imports: [LoadingStateDirective, TranslateDirective, FolderTreeComponent, NgClass, SmallSpinnerComponent]
})
export class DocumentCopyComponent implements OnInit, OnDestroy {
  public selected$ = new Subject<string>();

  protected loadingState: LoadingState = { isLoading: true };
  protected folderTree: FolderTreeEntry[] = [];
  protected selectedNode?: FolderTreeEntry;
  protected selectedTournamentId?: string;
  protected isLoadingDocuments = false;
  protected documents: DocumentDto[] = [];

  private readonly tournamentId$ = new Subject<string>();

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

    this.tournamentId$
      .pipe(
        tap(() => (this.isLoadingDocuments = true)),
        switchMap((id) => this.turnierplanApi.invoke(getDocuments, { tournamentId: id }))
      )
      .subscribe({
        next: (documents) => {
          this.documents = documents;
          this.documents.sort((a, b) => a.name.localeCompare(b.name));
          this.isLoadingDocuments = false;
        },
        error: (error) => {
          this.modal.dismiss({ isApiError: true, apiError: error as unknown });
        }
      });
  }

  public ngOnDestroy(): void {
    this.tournamentId$.complete();
  }

  protected selectTreeEntry(id: string): void {
    this.selectedNode = this.folderTree.find((x) => x.id === id);
    this.selectedTournamentId = undefined;
    this.documents = [];
  }

  protected selectTournament(id: string): void {
    this.selectedTournamentId = id;
    this.tournamentId$.next(id);
  }

  protected selectDocument(id: string): void {
    this.loadingState = { isLoading: true };
    this.selected$.next(id);
    this.selected$.complete();
  }
}
