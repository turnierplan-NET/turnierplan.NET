import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';

import { DocumentType } from '../../../api';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { getDocumentName } from '../../helpers/document-name';

type DocumentTypeEntry = {
  displayName: string;
  type: DocumentType;
};

@Component({
  templateUrl: './document-select.component.html',
  styleUrl: './document-select.component.scss'
})
export class DocumentSelectComponent implements OnInit {
  public selected$ = new Subject<DocumentType>();

  protected loadingState: LoadingState = { isLoading: false };
  protected documentTypes: DocumentTypeEntry[] = [];

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    // Translate the document types using the TranslateService so a name-based alphabetical sorting can be done.
    this.documentTypes = Object.values(DocumentType).map((type) => ({
      displayName: getDocumentName(type, this.translateService),
      type: type
    }));
    this.documentTypes.sort((a, b) => a.displayName.localeCompare(b.displayName));
  }

  protected selected(type: DocumentType): void {
    this.loadingState = { isLoading: true };
    this.selected$.next(type);
    this.selected$.complete();
  }
}
