import { Component } from '@angular/core';
import { PageFrameComponent, PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { Actions } from '../../../generated/actions';
import { ResourcePlannerDto } from '../../../api/models/resource-planner-dto';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { of, Subject, switchMap, takeUntil } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { TitleService } from '../../services/title.service';
import { getResourcePlanner } from '../../../api/fn/resource-planners/get-resource-planner';
import { RenameButtonComponent } from '../../components/rename-button/rename-button.component';
import { DeleteWidgetComponent } from '../../components/delete-widget/delete-widget.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { RbacWidgetComponent } from '../../components/rbac-widget/rbac-widget.component';

@Component({
  imports: [
    PageFrameComponent,
    LoadingStateDirective,
    RenameButtonComponent,
    DeleteWidgetComponent,
    IsActionAllowedDirective,
    RbacWidgetComponent
  ],
  templateUrl: './view-resource-planner.component.html'
})
export class ViewResourcePlannerComponent {
  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected resourcePlanner?: ResourcePlannerDto;

  protected currentPage = 0;
  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.ViewResourcePlanner.Pages.Resources',
      icon: 'bi-box-seam'
    },
    {
      id: 1,
      title: 'Portal.ViewResourcePlanner.Pages.Views',
      icon: 'bi-display'
    },
    {
      id: 2,
      title: 'Portal.ViewResourcePlanner.Pages.Settings',
      icon: 'bi-gear',
      authorization: Actions.GenericWrite
    }
  ];

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly route: ActivatedRoute,
    private readonly titleService: TitleService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const resourcePlannerId = params.get('id');
          if (resourcePlannerId === null) {
            this.loadingState = { isLoading: false };
            return of();
          }
          this.loadingState = { isLoading: true };
          return this.turnierplanApi.invoke(getResourcePlanner, { id: resourcePlannerId });
        })
      )
      .subscribe({
        next: (resourcePlanner) => {
          this.resourcePlanner = resourcePlanner;
          this.titleService.setTitleFrom(resourcePlanner);
          this.loadingState = { isLoading: false };
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

  protected togglePage(number: number): void {
    this.currentPage = number;
  }

  protected renameResourcePlanner(name: string): void {
    alert('Rename not implemented yet'); // TODO: Implement
  }

  protected deleteResourcePlanner(): void {
    alert('Delete not implemented yet'); // TODO: Implement
  }
}
