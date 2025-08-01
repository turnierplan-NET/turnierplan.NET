import { Component, Input } from '@angular/core';
import { PlanningRealmDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TournamentClassDialogComponent } from '../tournament-class-dialog/tournament-class-dialog.component';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';

@Component({
  standalone: false,
  selector: 'tp-tournament-classes-manager',
  templateUrl: './tournament-class-manager.component.html'
})
export class TournamentClassManagerComponent {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  protected readonly Actions = Actions;

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly modalService: NgbModal
  ) {}

  protected getNumberOfReferencingLinks(id: number): number {
    return this.planningRealm.invitationLinks.filter((link) => link.entries.some((entry) => entry.tournamentClassId == id)).length;
  }

  protected editTournamentClass(id: number): void {
    const ref = this.modalService.open(TournamentClassDialogComponent, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    (ref.componentInstance as TournamentClassDialogComponent).init(this.planningRealm, id);

    ref.closed.subscribe({
      next: (result) => {
        this.updatePlanningRealm((planningRealm) => {
          const tournamentClass = planningRealm.tournamentClasses.find((x) => x.id == id);

          if (!tournamentClass) {
            return false;
          }

          tournamentClass.name = result.name.trim();
          tournamentClass.maxTeamCount = result.maxTeamCount;

          return true;
        });
      }
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
}
