import { Component } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { TournamentClassDto } from '../../../api/models/tournament-class-dto';
import { CreateApplicationTeamEndpointRequest } from '../../../api/models/create-application-team-endpoint-request';

@Component({
  imports: [ActionButtonComponent, TranslateDirective, FormsModule],
  templateUrl: './manage-applications-add-team.component.html'
})
export class ManageApplicationsAddTeamComponent {
  protected tournamentClasses: TournamentClassDto[] = [];
  protected teamName: string = '';
  protected tournamentClassId: number = 0;
  protected showError = false;

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(planningRealm: PlanningRealmDto): void {
    this.tournamentClasses = [...planningRealm.tournamentClasses].sort((a, b) => a.name.localeCompare(b.name));
    this.tournamentClassId = this.tournamentClasses[0].id;
  }

  protected confirm(): void {
    if (this.teamName.trim().length === 0) {
      this.showError = true;
      return;
    }

    const result: CreateApplicationTeamEndpointRequest = {
      teamName: this.teamName.trim(),
      tournamentClassId: this.tournamentClassId
    };

    this.modal.close(result);
  }
}
