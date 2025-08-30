import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { TranslateService, TranslateDirective } from '@ngx-translate/core';
import { BehaviorSubject, combineLatestWith, delay, distinctUntilChanged, map, Subject, switchMap, tap } from 'rxjs';

import { ApiKeysService } from '../../../api';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { AlertComponent } from '../alert/alert.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { NgxEchartsDirective } from 'ngx-echarts';
import { EChartsCoreOption } from 'echarts/core';

@Component({
  selector: 'tp-api-key-usage',
  templateUrl: './api-key-usage.component.html',
  imports: [TranslateDirective, FormsModule, ActionButtonComponent, AlertComponent, SmallSpinnerComponent, NgxEchartsDirective]
})
export class ApiKeyUsageComponent implements OnDestroy {
  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly reload$ = new BehaviorSubject<void>(undefined);

  protected isLoading = true;
  protected timeRange: number = 7;
  protected showNoRequestsNotification = false;
  protected apiKeyUsageChart: EChartsCoreOption = {};

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
            labels.push(formatDate(bucketStart, 'short', translateService.getCurrentLang()));
            values.push(bucketCount);
            this.totalCount += bucketCount;
          }

          this.isLoading = false;

          this.apiKeyUsageChart = {
            tooltip: {
              trigger: 'axis'
            },
            grid: {
              top: '20',
              left: '20',
              right: '20',
              bottom: '20',
              containLabel: true
            },
            xAxis: {
              type: 'category',
              data: labels
            },
            yAxis: {
              type: 'value'
            },
            series: [
              {
                name: translateService.instant('Portal.ApiKeyUsage.Legend') as string,
                data: values,
                type: 'bar',
                color: '#16811a'
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
