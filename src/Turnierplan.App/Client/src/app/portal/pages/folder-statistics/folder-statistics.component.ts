import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService, TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { of, Subject, switchMap, takeUntil, tap } from 'rxjs';

import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { NgTemplateOutlet, NgStyle, DecimalPipe, PercentPipe } from '@angular/common';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { NgxEchartsDirective } from 'ngx-echarts';
import { EChartsCoreOption } from 'echarts/core';
import { FolderStatisticsDto } from '../../../api/models/folder-statistics-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getFolderStatistics } from '../../../api/fn/folders/get-folder-statistics';

@Component({
  templateUrl: './folder-statistics.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    TranslateDirective,
    NgTemplateOutlet,
    NgStyle,
    TooltipIconComponent,
    DecimalPipe,
    PercentPipe,
    TranslatePipe,
    NgxEchartsDirective
  ]
})
export class FolderStatisticsComponent implements OnInit, OnDestroy {
  protected loadingState: LoadingState = { isLoading: true };
  protected folderName: string = '';
  protected organizationId?: string;
  protected statistics?: FolderStatisticsDto;
  protected locale: string;
  protected goalDistributionExcludedTournaments?: string;

  protected outcomesBarChart: EChartsCoreOption = {};
  protected pageViewsBarChart: EChartsCoreOption = {};

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly route: ActivatedRoute,
    private readonly titleService: TitleService,
    private readonly translateService: TranslateService
  ) {
    this.locale = translateService.getCurrentLang();
  }

  public ngOnInit(): void {
    this.translateService.onLangChange.pipe(takeUntil(this.destroyed$)).subscribe({ next: (lang) => (this.locale = lang.lang) });

    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        tap(() => (this.loadingState = { isLoading: true })),
        switchMap((params) => {
          const folderId = params.get('id') ?? undefined;

          if (!folderId) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }

          this.loadingState = { isLoading: true };

          return this.turnierplanApi.invoke(getFolderStatistics, { id: folderId });
        })
      )
      .subscribe({
        next: (statistics) => {
          if (statistics) {
            this.folderName = statistics.folderName;
            this.organizationId = statistics.organizationId;
            this.titleService.setCompoundTitle(statistics.folderName);

            if (statistics.goalDistributionExcludedTournaments.length > 0) {
              const names = [...statistics.goalDistributionExcludedTournaments.map((x) => x.tournamentName)];
              names.sort((a, b) => a.localeCompare(b));
              this.goalDistributionExcludedTournaments = names.join(', ');
            } else {
              this.goalDistributionExcludedTournaments = undefined;
            }

            this.loadingState = { isLoading: false };
            this.statistics = statistics;
            this.generateCharts();
          }
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  private generateCharts(): void {
    if (!this.statistics) {
      return;
    }

    const commonOptions = {
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'none'
        }
      },
      grid: {
        top: '20',
        left: '20',
        right: '20',
        bottom: '20',
        containLabel: true
      }
    };

    this.outcomesBarChart = {
      ...commonOptions,
      xAxis: {
        type: 'category',
        data: this.statistics.outcomeDistribution.map((x) => `${x.outcome.scoreA}:${x.outcome.scoreB}`)
      },
      yAxis: {
        type: 'value'
      },
      series: [
        {
          name: this.translateService.instant('Portal.FolderStatistics.NumberOfMatches') as string,
          data: this.statistics.outcomeDistribution.map((x) => x.count),
          type: 'bar',
          color: '#16811a'
        }
      ]
    };

    this.pageViewsBarChart = {
      ...commonOptions,
      xAxis: {
        type: 'category',
        data: this.statistics.tournamentPageViews.map((x) => x.tournamentName),
        axisLabel: {
          rotate: -45
        }
      },
      yAxis: {
        type: 'value'
      },
      series: [
        {
          name: this.translateService.instant('Portal.FolderStatistics.NumberOfPageViews') as string,
          data: this.statistics.tournamentPageViews.map((x) => x.publicPageViews),
          type: 'bar',
          color: '#16811a'
        }
      ]
    };
  }
}
