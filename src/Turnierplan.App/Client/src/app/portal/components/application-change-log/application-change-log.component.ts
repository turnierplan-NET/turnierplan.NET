import { Component, OnDestroy } from '@angular/core';
import { ApplicationDto } from '../../../api/models/application-dto';
import { ApplicationChangeLogDto } from '../../../api/models/application-change-log-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { Observable, Subject } from 'rxjs';
import { getApplicationChangeLog } from '../../../api/fn/applications/get-application-change-log';

@Component({
  selector: 'tp-application-change-log',
  imports: [],
  templateUrl: './application-change-log.component.html',
  styleUrl: './application-change-log.component.scss'
})
export class ApplicationChangeLogComponent implements OnDestroy {
  protected isLoadingChangeLog = true;
  protected changeLog: ApplicationChangeLogDto[] = [];

  private readonly errorSubject$ = new Subject<unknown>();

  constructor(private readonly turnierplanApi: TurnierplanApi) {}

  public get error$(): Observable<unknown> {
    return this.errorSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.errorSubject$.complete();
  }

  public init(planningRealmId: string, application: ApplicationDto): void {
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
}
