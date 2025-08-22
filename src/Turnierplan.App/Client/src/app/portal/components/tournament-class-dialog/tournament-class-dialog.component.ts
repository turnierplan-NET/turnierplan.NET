import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PlanningRealmDto } from '../../../api';

@Component({
  standalone: false,
  templateUrl: './tournament-class-dialog.component.html'
})
export class TournamentClassDialogComponent {
  protected isInitialized = false;
  protected initialName: string = '';
  protected currentNumberOfTeams: number = 0;
  protected isClassReferencedByInvitationLink: boolean = false;
  protected nameInvalid = false;

  protected name: string = '';

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(planningRealm: PlanningRealmDto, tournamentClassId: number): void {
    const tournamentClass = planningRealm.tournamentClasses.find((x) => x.id === tournamentClassId);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${tournamentClassId} does not exist in planning realm.`);
    }

    this.name = tournamentClass.name;

    this.initialName = tournamentClass.name;
    this.currentNumberOfTeams = tournamentClass.numberOfTeams;
    this.isClassReferencedByInvitationLink = planningRealm.invitationLinks.some((link) =>
      link.entries.some((entry) => entry.tournamentClassId === tournamentClassId)
    );

    this.isInitialized = true;
  }

  protected applyChanges(): void {
    this.nameInvalid = false;

    const trimmedName = this.name.trim();
    if (trimmedName.length === 0) {
      this.nameInvalid = true;
      return;
    }

    if (trimmedName === this.initialName.trim()) {
      this.modal.dismiss();
    } else {
      this.modal.close({
        name: trimmedName
      });
    }
  }
}
