import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ChartConfiguration, ChartData } from 'chart.js';
import { BehaviorSubject, combineLatestWith, delay, distinctUntilChanged, map, Subject, switchMap, tap } from 'rxjs';

import { ApiKeysService } from '../../../api';

@Component({
  standalone: false,
  selector: 'tp-api-key-usage',
  templateUrl: './api-key-usage.component.html'
})
export class ApiKeyUsageComponent implements OnDestroy {
  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly chartOptions: ChartConfiguration<'bar'>['options'] = {};
  protected readonly reload$ = new BehaviorSubject<void>(undefined);

  protected isLoading = true;
  protected timeRange: number = 7;
  protected showNoRequestsNotification = false;

  protected chartData: ChartData<'bar'> = {
    labels: [],
    datasets: []
  };

  protected totalCount = 0;

  private readonly apiKeyId$ = new Subject<string>();

  constructor(apiKeyClient: ApiKeysService, translateService: TranslateService) {
    this.apiKeyId$
      .pipe(
        distinctUntilChanged(),
        combineLatestWith(this.reload$),
        map(([x]) => x),
        tap(() => (this.isLoading = true)),
        delay(300),
        switchMap((id) => apiKeyClient.getApiKeyUsage({ rangeDays: this.timeRange, id: id }))
      )
      .subscribe({
        next: (data) => {
          const labels = [];
          const values = [];

          this.showNoRequestsNotification = !data.entries.some((x) => x.count > 0);
          this.totalCount = 0;

          for (let i = 0; i < data.bucketCount; i++) {
            const bucketStart = new Date(new Date(data.timeFrameStart).getTime() + i * data.bucketWidthSeconds * 1000);
            const bucketCount = data.entries.find((x) => x.bucketIndex === i)?.count ?? 0;
            labels.push(formatDate(bucketStart, 'short', translateService.currentLang));
            values.push(bucketCount);
            this.totalCount += bucketCount;
          }

          this.isLoading = false;
          this.chartData = {
            labels: labels,
            datasets: [
              {
                data: values,
                backgroundColor: '#16811a',
                borderColor: '#0e4210',
                hoverBackgroundColor: '#279f2b',
                label: translateService.instant('Portal.ApiKeyUsage.Legend') as string
              }
            ]
          };
        },
        error: (error) => this.errorOccured.emit(error)
      });
  }

  @Input()
  public set apiKeyId(value: string) {
    this.apiKeyId$.next(value);
  }

  public ngOnDestroy(): void {
    this.apiKeyId$.complete();
    this.reload$.complete();
  }
}
