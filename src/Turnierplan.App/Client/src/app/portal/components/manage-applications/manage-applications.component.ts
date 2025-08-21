import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { ApplicationsFilter } from '../../models/applications-filter';
import { ReplaySubject, switchMap } from 'rxjs';
import { ApplicationsService, PlanningRealmDto } from '../../../api';
import { PaginationResultDtoOfApplicationDto } from '../../../api/models/pagination-result-dto-of-application-dto';

@Component({
  standalone: false,
  selector: 'tp-manage-applications',
  templateUrl: './manage-applications.component.html'
})
export class ManageApplicationsComponent implements OnDestroy {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected result?: PaginationResultDtoOfApplicationDto;

  private readonly filter$ = new ReplaySubject<ApplicationsFilter>();

  constructor(private readonly applicationsService: ApplicationsService) {
    this.filter$
      .pipe(
        switchMap((filter) => this.applicationsService.getApplications({ planningRealmId: this.planningRealm.id, page: 0, pageSize: 20 }))
      )
      .subscribe({
        next: (result) => {
          this.result = result;
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  @Input()
  public set filter(value: ApplicationsFilter) {
    this.filter$.next(value);
  }

  public ngOnDestroy(): void {
    this.filter$.complete();
  }
}
