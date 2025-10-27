import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ComponentRef,
  InjectionToken,
  Injector,
  Type,
  ViewChild,
  ViewContainerRef
} from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, Subject } from 'rxjs';

import { NotificationService } from '../../../core/services/notification.service';
import { DocumentConfiguration } from '../../models/document-configuration';
import { TranslateDirective } from '@ngx-translate/core';
import { ActionButtonComponent } from '../action-button/action-button.component';

export abstract class DocumentConfigComponent<T> {
  public abstract submit: Subject<void>;
  public abstract getConfig(): T;
  public abstract isValid(): boolean;
}

export const CURRENT_CONFIGURATION = new InjectionToken<string>('CURRENT_CONFIGURATION');

@Component({
  templateUrl: './document-config-frame.component.html',
  imports: [TranslateDirective, ActionButtonComponent]
})
export class DocumentConfigFrameComponent implements AfterViewInit {
  @ViewChild('configContainer', { read: ViewContainerRef })
  protected configContainer!: ViewContainerRef;

  protected initError = false;
  protected isSaving = false;

  private documentId?: string;
  private initialConfiguration?: { component: Type<DocumentConfigComponent<DocumentConfiguration>>; config: DocumentConfiguration };
  private configComponent?: ComponentRef<DocumentConfigComponent<DocumentConfiguration>>;
  private saveConfig?: (config: DocumentConfiguration) => Observable<void>;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly injector: Injector,
    private readonly notificationService: NotificationService,
    private readonly changeDetector: ChangeDetectorRef
  ) {}

  public ngAfterViewInit(): void {
    if (this.initialConfiguration === undefined) {
      this.initError = true;
      this.changeDetector.detectChanges();
      return;
    }

    const injector = Injector.create({
      parent: this.injector,
      providers: [
        {
          provide: CURRENT_CONFIGURATION,
          useValue: this.initialConfiguration.config
        }
      ]
    });

    this.configComponent = this.configContainer.createComponent(this.initialConfiguration.component, { injector: injector });
    this.configComponent.instance.submit.subscribe(() => this.saveChanges());
    this.initialConfiguration = undefined;

    this.changeDetector.detectChanges();
  }

  public init<T extends DocumentConfiguration>(
    component: Type<DocumentConfigComponent<T>>,
    documentId: string,
    config: T,
    saveConfigFn: (config: DocumentConfiguration) => Observable<void>
  ): void {
    // The <ng-content> element can only be accessed after the view has been initialized. Therefore, we must temporarily
    // store the component type and existing configuration until the ngAfterViewInit() function is called.

    this.documentId = documentId;
    this.initialConfiguration = { component: component, config: config };
    this.saveConfig = saveConfigFn;
  }

  protected saveChanges(): void {
    if (this.isSaving || !this.saveConfig) {
      return;
    }

    const configComponent = this.configComponent?.instance;
    const isValid = configComponent?.isValid();

    if (isValid !== true) {
      this.notificationService.showNotification(
        'warning',
        'Portal.ViewTournament.Documents.ConfigureModal.ValidationErrorsToast.Title',
        'Portal.ViewTournament.Documents.ConfigureModal.ValidationErrorsToast.Message'
      );

      return;
    }

    const updatedConfig = configComponent?.getConfig();

    if (updatedConfig === undefined || this.documentId === undefined) {
      return;
    }

    this.isSaving = true;

    this.saveConfig(updatedConfig).subscribe({
      next: () => {
        this.notificationService.showNotification(
          'success',
          'Portal.ViewTournament.Documents.ConfigureModal.SuccessToast.Title',
          'Portal.ViewTournament.Documents.ConfigureModal.SuccessToast.Message'
        );
        this.modal.close();
      },
      error: () => {
        this.notificationService.showNotification(
          'danger',
          'Portal.ViewTournament.Documents.ConfigureModal.UpdateFailedToast.Title',
          'Portal.ViewTournament.Documents.ConfigureModal.UpdateFailedToast.Message'
        );
        this.isSaving = false;
      }
    });
  }
}
