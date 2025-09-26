import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective } from '@ngx-translate/core';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { AsyncPipe } from '@angular/common';
import { LabelComponent } from '../label/label.component';
import { RenameButtonComponent } from '../rename-button/rename-button.component';

@Component({
  selector: 'tp-labels-manager',
  imports: [TranslateDirective, AsyncPipe, LabelComponent, RenameButtonComponent],
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
}
