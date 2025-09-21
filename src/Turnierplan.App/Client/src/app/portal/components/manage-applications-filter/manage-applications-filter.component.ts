import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ApplicationsFilter, defaultApplicationsFilter, InvitationLinkFilterValue } from '../../models/applications-filter';
import { MultiSelectFilterOption, MultiSelectFilterComponent } from '../multi-select-filter/multi-select-filter.component';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-manage-applications-filter',
  templateUrl: './manage-applications-filter.component.html',
  imports: [FormsModule, MultiSelectFilterComponent, ActionButtonComponent, NgbTooltip, TranslatePipe]
})
export class ManageApplicationsFilterComponent {
  protected searchTerm: string = '';
  protected tournamentClass: number[] = [];
  protected invitationLink: InvitationLinkFilterValue[] = [];

  protected tournamentClassFilterOptions: MultiSelectFilterOption[] = [];
  protected invitationLinkFilterOptions: MultiSelectFilterOption[] = [];
  protected invitationLinkSpecialValue: InvitationLinkFilterValue = 'none';

  @Input()
  public set planningRealm(value: PlanningRealmDto) {
    this.tournamentClassFilterOptions = value.tournamentClasses.map((x) => ({ value: x.id, label: x.name }));
    this.invitationLinkFilterOptions = value.invitationLinks.map((x) => ({ value: x.id, label: x.name }));
  }

  @Input()
  public set filter(value: ApplicationsFilter) {
    this.searchTerm = value.searchTerm;
    this.tournamentClass = value.tournamentClass;
    this.invitationLink = value.invitationLink;
  }

  @Output()
  public filterChange = new EventEmitter<ApplicationsFilter>();

  protected emitFilter() {
    this.filterChange.emit({
      searchTerm: this.searchTerm,
      tournamentClass: this.tournamentClass,
      invitationLink: this.invitationLink
    });
  }

  protected resetAndEmitFilter(): void {
    this.filter = defaultApplicationsFilter;
    this.emitFilter();
  }
}
