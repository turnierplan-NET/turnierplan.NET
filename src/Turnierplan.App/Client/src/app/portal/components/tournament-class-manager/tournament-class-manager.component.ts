import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PlanningRealmDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective } from '@ngx-translate/core';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { AsyncPipe } from '@angular/common';

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
  public planningRealm!: PlanningRealmDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();

  protected readonly Actions = Actions;

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly modalService: NgbModal
  ) {}

  protected getNumberOfReferencingLinks(id: number): number {
    return this.planningRealm.invitationLinks.filter((link) => link.entries.some((entry) => entry.tournamentClassId == id)).length;
  }

  protected renameTournamentClass(id: number, name: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const tournamentClass = planningRealm.tournamentClasses.find((x) => x.id == id);

      if (!tournamentClass) {
        return false;
      }

      tournamentClass.name = name;

      return true;
    });
  }

  protected deleteTournamentClass(id: number): void {
    this.updatePlanningRealm((planningRealm) => {
      const index = planningRealm.tournamentClasses.findIndex((x) => x.id === id);

      if (index === -1) {
        return false;
      }

      planningRealm.tournamentClasses.splice(index, 1);

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
      tournamentClass: [id]
    });
  }
}
