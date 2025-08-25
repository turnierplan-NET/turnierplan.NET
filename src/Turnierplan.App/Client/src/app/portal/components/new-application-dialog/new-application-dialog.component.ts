import { Component } from '@angular/core';
import { PlanningRealmDto } from '../../../api';

@Component({
  standalone: false,
  templateUrl: './new-application-dialog.component.html'
})
export class NewApplicationDialogComponent {
  public init(planningRealm: PlanningRealmDto): void {}
}
