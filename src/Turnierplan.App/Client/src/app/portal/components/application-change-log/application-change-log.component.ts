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
import { ApplicationChangeLogEntryComponent } from '../application-change-log-entry/application-change-log-entry.component';

@Component({
  selector: 'tp-application-change-log',
  imports: [TranslateDirective, LoadingStateDirective, TranslateDatePipe, ApplicationChangeLogEntryComponent],
  templateUrl: './application-change-log.component.html',
  styleUrl: './application-change-log.component.scss'
})
export class ApplicationChangeLogComponent implements OnDestroy {
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
        this.changeLog.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
        this.isLoadingChangeLog = false;
      },
      error: (error) => {
        this.errorSubject$.next(error);
      }
    });
  }
}
