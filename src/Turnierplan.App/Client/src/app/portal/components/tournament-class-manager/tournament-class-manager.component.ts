import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { UpdateTournamentPlannerFunc } from '../../pages/view-tournament-planner/view-tournament-planner.component';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective } from '@ngx-translate/core';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { AsyncPipe } from '@angular/common';
import { TournamentPlannerDto } from '../../../api/models/tournament-planner-dto';

@Component({
  selector: 'tp-tournament-classes-manager',
  templateUrl: './tournament-class-manager.component.html',
  imports: [
    TranslateDirective,
    IsActionAllowedDirective,
    ActionButtonComponent,
    RenameButtonComponent,
    DeleteButtonComponent,
    TooltipIconComponent,
    AsyncPipe
  ]
})
export class TournamentClassManagerComponent {
  @Input()
  public tournamentPlanner!: TournamentPlannerDto;

  @Input()
  public updateTournamentPlanner!: UpdateTournamentPlannerFunc;

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();

  protected readonly Actions = Actions;

  constructor(protected readonly authorizationService: AuthorizationService) {}

  protected getNumberOfReferencingLinks(id: number): number {
    return this.tournamentPlanner.invitationLinks.filter((link) => link.entries.some((entry) => entry.tournamentClassId == id)).length;
  }

  protected renameTournamentClass(id: number, name: string): void {
    this.updateTournamentPlanner((tournamentPlanner) => {
      const tournamentClass = tournamentPlanner.tournamentClasses.find((x) => x.id == id);

      if (!tournamentClass) {
        return false;
      }

      tournamentClass.name = name;

      return true;
    });
  }

  protected deleteTournamentClass(id: number): void {
    this.updateTournamentPlanner((tournamentPlanner) => {
      const index = tournamentPlanner.tournamentClasses.findIndex((x) => x.id === id);

      if (index === -1) {
        return false;
      }

      tournamentPlanner.tournamentClasses.splice(index, 1);

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
      tournamentClass: [id],
      label: []
    });
  }
}
