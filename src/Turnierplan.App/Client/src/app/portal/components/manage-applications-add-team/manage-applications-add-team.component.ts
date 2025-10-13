import { Component } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  imports: [ActionButtonComponent, TranslateDirective],
  templateUrl: './manage-applications-add-team.component.html'
})
export class ManageApplicationsAddTeamComponent {
  constructor(protected readonly modal: NgbActiveModal) {}

  public init(): void {}

  protected confirm(): void {}
}
