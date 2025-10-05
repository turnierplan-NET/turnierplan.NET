import { Component, OnDestroy } from '@angular/core';
import { ApplicationDto } from '../../../api/models/application-dto';
import { ApplicationChangeLogDto } from '../../../api/models/application-change-log-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { Observable, Subject } from 'rxjs';
import { getApplicationChangeLog } from '../../../api/fn/applications/get-application-change-log';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';
import { LoadingStateDirective } from '../../directives/loading-state.directive';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { ApplicationChangeLogType } from '../../../api/models/application-change-log-type';
import { ApplicationChangeLogProperty } from '../../../api/models/application-change-log-property';

@Component({
  selector: 'tp-application-change-log',
  imports: [TranslateDirective, LoadingStateDirective, TranslateDatePipe],
  templateUrl: './application-change-log.component.html',
  styleUrl: './application-change-log.component.scss'
})
export class ApplicationChangeLogComponent implements OnDestroy {
  protected readonly ApplicationChangeLogType = ApplicationChangeLogType;
  protected readonly ApplicationChangeLogProperty = ApplicationChangeLogProperty;

  protected isLoadingChangeLog = true;
  protected changeLog: ApplicationChangeLogDto[] = [];
  protected applicationCreatedAt: string = '';

  private readonly errorSubject$ = new Subject<unknown>();

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly turnierplanApi: TurnierplanApi
  ) {}

  public get error$(): Observable<unknown> {
    return this.errorSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.errorSubject$.complete();
  }

  public init(planningRealmId: string, application: ApplicationDto): void {
    this.applicationCreatedAt = application.createdAt;

    this.turnierplanApi.invoke(getApplicationChangeLog, { planningRealmId: planningRealmId, applicationId: application.id }).subscribe({
      next: (result) => {
        this.changeLog = result;
        this.isLoadingChangeLog = false;
      },
      error: (error) => {
        this.errorSubject$.next(error);
      }
    });
  }

  protected getPropertyValue(entry: ApplicationChangeLogDto, type: ApplicationChangeLogProperty): string {
    return entry.properties.find((x) => x.type === type)?.value ?? '';
  }

  protected getChangeLogIcon(type: ApplicationChangeLogType): string {
    switch (type) {
      case ApplicationChangeLogType.NotesChanged:
      case ApplicationChangeLogType.CommentChanged:
      case ApplicationChangeLogType.ContactChanged:
      case ApplicationChangeLogType.ContactEmailChanged:
      case ApplicationChangeLogType.ContactTelephoneChanged:
      case ApplicationChangeLogType.TeamRenamed:
        return 'bi-pencil-square';
      case ApplicationChangeLogType.TeamAdded:
        return 'bi-plus-square';
      case ApplicationChangeLogType.TeamRemoved:
        return 'bi-dash-square';
      case ApplicationChangeLogType.LabelAdded:
      case ApplicationChangeLogType.LabelRemoved:
        return 'bi-tags';
    }
  }
}
