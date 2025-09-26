import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective } from '@ngx-translate/core';

@Component({
  selector: 'tp-labels-manager',
  imports: [TranslateDirective],
  templateUrl: './labels-manager.component.html'
})
export class LabelsManagerComponent {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();
}
