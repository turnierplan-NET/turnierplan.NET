import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ApplicationsFilter, defaultApplicationsFilter } from '../../models/applications-filter';

@Component({
  standalone: false,
  selector: 'tp-manage-applications-filter',
  templateUrl: './manage-applications-filter.component.html'
})
export class ManageApplicationsFilterComponent {
  protected searchTerm: string = '';

  @Input()
  public set filter(value: ApplicationsFilter) {
    this.searchTerm = value.searchTerm;
  }

  @Output()
  public filterChange = new EventEmitter<ApplicationsFilter>();

  protected emitFilter() {
    this.filterChange.emit({
      searchTerm: this.searchTerm
    });
  }

  protected resetAndEmitFilter(): void {
    this.filter = defaultApplicationsFilter;
    this.emitFilter();
  }
}
