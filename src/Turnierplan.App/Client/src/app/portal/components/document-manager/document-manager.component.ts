import { Component, EventEmitter, Injector, Input, Output, Type, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { PdfJsViewerComponent } from 'ng2-pdfjs-viewer';
import { finalize, Observable, tap } from 'rxjs';

import { DocumentDto, DocumentsService, DocumentType, MatchPlanDocumentConfiguration, ReceiptsDocumentConfiguration } from '../../../api';
import { DocumentConfiguration } from '../../models/document-configuration';
import { DocumentConfigComponent, DocumentConfigFrameComponent } from '../document-config-frame/document-config-frame.component';
import { DocumentConfigMatchPlanComponent } from '../document-config-match-plan/document-config-match-plan.component';
import { DocumentConfigReceiptsComponent } from '../document-config-receipts/document-config-receipts.component';

@Component({
  selector: 'tp-document-manager',
  templateUrl: './document-manager.component.html',
  styleUrls: ['document-manager.component.scss']
})
export class DocumentManagerComponent {
  @Input()
  public tournamentName: string = '';

  @Input()
  public documents: DocumentDto[] = [];

  @Input()
  public recentDocumentId: string | undefined;

  @Output()
  public deleteClick = new EventEmitter<string>();

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  @ViewChild('previewPdfViewer')
  protected previewPdfViewer!: PdfJsViewerComponent;

  protected currentlyViewedDocumentId: string | undefined;
  protected displayPdfViewer: boolean = false;
  protected currentlyLoadingConfiguration?: string;
  protected currentlyDownloading?: string;
  protected currentlyLoadingPreview?: string;
  protected currentlyUpdatingName?: string;

  private readonly configComponents = new Map<DocumentType, Type<DocumentConfigComponent<DocumentConfiguration>>>();

  constructor(
    private readonly injector: Injector,
    private readonly documentService: DocumentsService,
    private readonly translateService: TranslateService,
    private readonly modalService: NgbModal
  ) {
    this.configComponents.set(DocumentType.MatchPlan, DocumentConfigMatchPlanComponent);
    this.configComponents.set(DocumentType.Receipts, DocumentConfigReceiptsComponent);
  }

  protected get downloadOrPreviewDisabled(): boolean {
    return this.currentlyDownloading !== undefined || this.currentlyLoadingPreview !== undefined;
  }

  protected canConfigure(type: DocumentType | string): boolean {
    return this.configComponents.has(type as DocumentType);
  }

  protected configureDocument(id: string): void {
    const document = this.documents.find((x) => x.id === id);
    if (document === undefined) {
      return;
    }

    const configComponent = this.configComponents.get(document.type);
    if (configComponent === undefined) {
      return;
    }

    this.currentlyLoadingConfiguration = id;

    this.getDocumentConfig(document)
      .pipe(finalize(() => (this.currentlyLoadingConfiguration = undefined)))
      .subscribe({
        next: (currentConfig: DocumentConfiguration) => {
          const ref = this.modalService.open(DocumentConfigFrameComponent, {
            size: 'lg',
            fullscreen: 'lg',
            centered: true,
            injector: this.injector,
            backdrop: 'static'
          });

          const component = ref.componentInstance as DocumentConfigFrameComponent;
          component.init(configComponent, id, currentConfig, this.getDocumentConfigSaveFunction(document));

          if (this.currentlyViewedDocumentId === id) {
            ref.closed.subscribe(() => this.loadDocumentPreview(id, document.name));
          }
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected downloadDocument(id: string): void {
    if (this.downloadOrPreviewDisabled) {
      return;
    }

    const documentName = this.documents.find((x) => x.id === id)?.name;

    if (documentName) {
      this.currentlyDownloading = id;
      const fileName = this.getDocumentFileName(documentName);

      this.documentService
        .getDocumentPdf({ id: id, languageCode: this.translateService.currentLang, timeZone: this.getTimeZoneName() })
        .subscribe({
          next: (result) => {
            this.documents.find((x) => x.id === id)!.generationCount += 1;

            const a = document.createElement('a');
            a.href = URL.createObjectURL(result);
            a.download = fileName;
            a.click();

            this.currentlyDownloading = undefined;
          },
          error: (error) => {
            this.errorOccured.emit(error);
          }
        });
    }
  }

  protected previewDocument(id: string): void {
    if (this.downloadOrPreviewDisabled || this.currentlyViewedDocumentId === id) {
      return;
    }

    const documentName = this.documents.find((x) => x.id === id)?.name;

    if (documentName) {
      this.loadDocumentPreview(id, documentName);
    }
  }

  protected renameDocument(id: string, name: string): void {
    if (this.currentlyUpdatingName) {
      return;
    }

    this.currentlyUpdatingName = id;

    this.documentService.setDocumentName({ id: id, body: { name: name } }).subscribe({
      next: () => {
        const document = this.documents.find((x) => x.id === id);
        if (document) {
          document.name = name;
        }
        this.currentlyUpdatingName = undefined;
      },
      error: (error) => {
        this.errorOccured.emit(error);
      }
    });
  }

  protected deleteDocument(id: string): void {
    if (id === this.currentlyViewedDocumentId) {
      this.displayPdfViewer = false;
    }
    this.deleteClick.emit(id);
  }

  private loadDocumentPreview(id: string, documentName: string): void {
    this.currentlyLoadingPreview = id;
    const fileName = this.getDocumentFileName(documentName);

    this.displayPdfViewer = true;
    this.currentlyViewedDocumentId = id;

    this.documentService
      .getDocumentPdf({ id: id, languageCode: this.translateService.currentLang, timeZone: this.getTimeZoneName() })
      .pipe(
        tap(() => {
          this.currentlyLoadingPreview = undefined;
          this.documents.find((x) => x.id === id)!.generationCount += 1;
        })
      )
      .subscribe({
        next: (result) => {
          this.previewPdfViewer.pdfSrc = result;
          this.previewPdfViewer.zoom = 'page-fit';
          this.previewPdfViewer.downloadFileName = fileName;
          this.previewPdfViewer.refresh();
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  private getDocumentFileName(name: string): string {
    const fileNameSanitizeRegex = /[^.A-Za-z0-9Ä-Öä-öß _-]/g;
    return `${name} - ${this.tournamentName}.pdf`.replace(fileNameSanitizeRegex, '_');
  }

  private getDocumentConfig(document: DocumentDto): Observable<DocumentConfiguration> {
    switch (document.type) {
      case DocumentType.MatchPlan:
        return this.documentService.getMatchPlanDocumentConfiguration({ id: document.id });
      case DocumentType.Receipts:
        return this.documentService.getReceiptsDocumentConfiguration({ id: document.id });
    }

    throw new Error(`Document type '${document.type}' is currently not registered for fetching configuration.`);
  }

  private getDocumentConfigSaveFunction(document: DocumentDto): (config: DocumentConfiguration) => Observable<void> {
    switch (document.type) {
      case DocumentType.MatchPlan:
        return (config) =>
          this.documentService.setMatchPlanDocumentConfiguration({ id: document.id, body: config as MatchPlanDocumentConfiguration });
      case DocumentType.Receipts:
        return (config) =>
          this.documentService.setReceiptsDocumentConfiguration({ id: document.id, body: config as ReceiptsDocumentConfiguration });
    }

    throw new Error(`Document type '${document.type}' is currently not registered for saving configuration.`);
  }

  private getTimeZoneName(): string {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
  }
}
