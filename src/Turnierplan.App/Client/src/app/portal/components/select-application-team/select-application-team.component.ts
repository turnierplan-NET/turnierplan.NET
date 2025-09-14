import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import {
  ApplicationsService,
  PaginationResultDtoOfApplicationDto,
  PlanningRealmDto,
  PlanningRealmHeaderDto,
  PlanningRealmsService,
  PublicId
} from '../../../api';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { FormsModule } from '@angular/forms';
import { LocalStorageService } from '../../services/local-storage.service';
import { catchError, of, Subject, switchMap } from 'rxjs';
import { ManageApplicationsFilterComponent } from '../manage-applications-filter/manage-applications-filter.component';
import { ApplicationsFilter, applicationsFilterToQueryParameters, defaultApplicationsFilter } from '../../models/applications-filter';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';

export type SelectApplicationTeamResult = {
  name: string;
  planningRealmId: string;
  applicationTeamId: number;
};

@Component({
  selector: 'tp-select-application-team',
  imports: [TranslateDirective, SmallSpinnerComponent, FormsModule, ManageApplicationsFilterComponent, TooltipIconComponent],
  templateUrl: './select-application-team.component.html',
  styleUrl: './select-application-team.component.scss'
})
export class SelectApplicationTeamComponent implements OnInit, OnDestroy {
  protected isLoadingPlanningRealms = true;
  protected planningRealms?: PlanningRealmHeaderDto[];
  protected currentPlanningRealmId?: string;
  protected isLoadingPlanningRealmDetail = true;
  protected planningRealmDetail?: PlanningRealmDto;
  protected applicationsFilter: ApplicationsFilter = defaultApplicationsFilter;
  protected isLoadingApplications = true;
  protected applications?: PaginationResultDtoOfApplicationDto;

  @Input()
  public organizationId!: PublicId;

  @Output()
  public teamSelected = new EventEmitter<SelectApplicationTeamResult>();

  private planningRealmId$ = new Subject<string>();
  private loadApplications$ = new Subject<void>();

  constructor(
    private readonly planningRealmService: PlanningRealmsService,
    private readonly applicationsService: ApplicationsService,
    private readonly localStorageService: LocalStorageService
  ) {}

  public ngOnInit(): void {
    this.isLoadingPlanningRealms = true;

    this.planningRealmService.getPlanningRealms({ organizationId: this.organizationId }).subscribe({
      next: (result) => {
        this.planningRealms = result;
        this.isLoadingPlanningRealms = false;

        if (this.planningRealms.length > 0) {
          const currentId = this.localStorageService.getSelectApplicationTeamPlanningRealmId(this.organizationId);
          if (currentId && this.planningRealms.some((x) => x.id === currentId)) {
            this.currentPlanningRealmId = currentId;
            this.onPlanningRealmChange(currentId);
          } else {
            this.currentPlanningRealmId = this.planningRealms[0].id;
            this.onPlanningRealmChange(this.planningRealms[0].id);
          }
        }
      },
      error: () => {
        this.isLoadingPlanningRealms = false;
      }
    });

    this.planningRealmId$
      .pipe(
        switchMap((id) => {
          this.isLoadingPlanningRealmDetail = true;
          return this.planningRealmService.getPlanningRealm({ id: id });
        }),
        catchError(() => of(undefined))
      )
      .subscribe({
        next: (result) => {
          this.planningRealmDetail = result;
          this.isLoadingPlanningRealmDetail = false;
          this.loadApplications$.next();
        }
      });

    this.loadApplications$
      .pipe(
        switchMap(() => {
          this.isLoadingApplications = true;
          return this.applicationsService.getApplications({
            planningRealmId: this.planningRealmDetail!.id,
            page: 0,
            pageSize: 99,
            ...applicationsFilterToQueryParameters(this.applicationsFilter) // TODO: Filter applications that have no team link atm
          });
        }),
        catchError(() => of(undefined))
      )
      .subscribe({
        next: (result) => {
          this.isLoadingApplications = false;
          this.applications = result;
        }
      });
  }

  public ngOnDestroy(): void {
    this.planningRealmId$.complete();
  }

  protected onPlanningRealmChange(id: string): void {
    this.localStorageService.setSelectApplicationTeamPlanningRealmId(this.organizationId, id);
    this.applicationsFilter = this.localStorageService.getPlanningRealmApplicationsFilter(id);
    this.planningRealmId$.next(id);
  }

  protected onApplicationsFilterChange(filter: ApplicationsFilter): void {
    this.applicationsFilter = filter;

    if (this.planningRealmDetail) {
      this.localStorageService.setPlanningRealmApplicationsFilter(this.planningRealmDetail.id, filter);
    }

    this.loadApplications$.next();
  }
}
