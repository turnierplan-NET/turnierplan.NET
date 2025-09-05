import { Component } from '@angular/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { FormsModule } from '@angular/forms';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  imports: [ActionButtonComponent, FormsModule, TranslateDirective],
  templateUrl: './configure-tournament-add-team.component.html'
})
export class ConfigureTournamentAddTeamComponent {
  // TODO: This dialog can add new teams & select from planning realm

  constructor(protected readonly modal: NgbActiveModal) {}

  protected confirm(): void {}
}
