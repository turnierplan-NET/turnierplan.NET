import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { PublicId } from '../../../api';

export type SelectApplicationTeamResult = {
  name: string;
  planningRealmId: string;
  applicationTeamId: number;
};

@Component({
  selector: 'tp-select-application-team',
  imports: [TranslateDirective],
  templateUrl: './select-application-team.component.html'
})
export class SelectApplicationTeamComponent {
  @Input()
  public organizationId!: PublicId;

  @Output()
  public teamSelected = new EventEmitter<SelectApplicationTeamResult>();
}
