import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { UpdatePlanningRealmFunc, ViewPlanningRealmComponent } from '../../pages/view-planning-realm/view-planning-realm.component';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective } from '@ngx-translate/core';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { AsyncPipe } from '@angular/common';
import { LabelComponent } from '../label/label.component';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { NgbPopoverModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'tp-labels-manager',
  imports: [
    TranslateDirective,
    AsyncPipe,
    LabelComponent,
    RenameButtonComponent,
    ActionButtonComponent,
    IsActionAllowedDirective,
    NgbPopoverModule
  ],
  templateUrl: './labels-manager.component.html'
})
export class LabelsManagerComponent {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();

  protected readonly Actions = Actions;
  protected readonly availableColors = ViewPlanningRealmComponent.DefaultLabelColorCodes;

  constructor(protected readonly authorizationService: AuthorizationService) {}

  protected setLabelName(id: number, name: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const label = planningRealm.labels.find((x) => x.id == id);

      if (!label) {
        return false;
      }

      label.name = name;

      return true;
    });
  }

  protected setLabelDescription(id: number, description: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const label = planningRealm.labels.find((x) => x.id == id);

      if (!label) {
        return false;
      }

      label.description = description;

      return true;
    });
  }

  protected setLabelColor(id: any, color: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const label = planningRealm.labels.find((x) => x.id == id);

      if (!label) {
        return false;
      }

      // The color comes with the '#' CSS prefix
      label.colorCode = color.substring(1);

      return true;
    });
  }

  protected searchApplicationsClicked(id: number): void {
    if (id < 0) {
      return;
    }

    this.filterRequested.emit({
      searchTerm: '',
      invitationLink: [],
      tournamentClass: [],
      label: [id]
    });
  }
}
