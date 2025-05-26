import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ChartConfiguration, ChartData } from 'chart.js';
import { of, Subject, switchMap, takeUntil, tap } from 'rxjs';

import { FolderStatisticsDto, FoldersService } from '../../../api';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';

@Component({
  templateUrl: './folder-statistics.component.html'
})
export class FolderStatisticsComponent implements OnInit, OnDestroy {
  protected readonly barChartOptions: ChartConfiguration<'bar'>['options'] = {};

  protected loadingState: LoadingState = { isLoading: true };
  protected folderName: string = '';
  protected organizationId?: string;
  protected statistics?: FolderStatisticsDto;
  protected locale: string;
  protected goalDistributionExcludedTournaments?: string;

  protected outcomesBarChart: ChartData<'bar'> = {
    labels: [],
    datasets: []
  };

  protected pageViewsBarChart: ChartData<'bar'> = {
    labels: [],
    datasets: []
  };

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly folderService: FoldersService,
    private readonly titleService: TitleService,
    private readonly translateService: TranslateService
  ) {
    this.locale = translateService.currentLang;
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

          return this.folderService.getFolderStatistics({ id: folderId });
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

    this.outcomesBarChart = {
      labels: this.statistics.outcomeDistribution.map((x) => `${x.outcome.scoreA}:${x.outcome.scoreB}`),
      datasets: [
        {
          data: this.statistics.outcomeDistribution.map((x) => x.count),
          backgroundColor: '#16811a',
          borderColor: '#0e4210',
          hoverBackgroundColor: '#279f2b',
          label: this.translateService.instant('Portal.FolderStatistics.NumberOfMatches') as string
        }
      ]
    };

    this.pageViewsBarChart = {
      labels: this.statistics.tournamentPageViews.map((x) => x.tournamentName),
      datasets: [
        {
          data: this.statistics.tournamentPageViews.map((x) => x.publicPageViews),
          backgroundColor: '#16811a',
          borderColor: '#0e4210',
          hoverBackgroundColor: '#279f2b',
          label: this.translateService.instant('Portal.FolderStatistics.NumberOfPageViews') as string
        }
      ]
    };
  }
}
