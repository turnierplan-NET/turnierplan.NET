import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PlanningRealmDto, TournamentClassDto } from '../../../api';
import { max } from 'rxjs';

@Component({
  standalone: false,
  templateUrl: './tournament-class-dialog.component.html'
})
export class TournamentClassDialogComponent {
  protected isInitialized = false;
  protected initialName: string = '';
  protected initialMaxTeamCount?: number;
  protected currentNumberOfTeams: number = 0;
  protected isClassReferencedByInvitationLink: boolean = false;
  protected nameInvalid = false;

  protected name: string = '';
  protected maxTeamCount?: number;

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(planningRealm: PlanningRealmDto, tournamentClassId: number): void {
    const tournamentClass = planningRealm.tournamentClasses.find((x) => x.id === tournamentClassId);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${tournamentClassId} does not exist in planning realm.`);
    }

    this.name = tournamentClass.name;
    this.maxTeamCount = tournamentClass.maxTeamCount ?? undefined;

    this.initialName = tournamentClass.name;
    this.initialMaxTeamCount = tournamentClass.maxTeamCount ?? undefined;
    this.currentNumberOfTeams = tournamentClass.numberOfTeams;
    this.isClassReferencedByInvitationLink = planningRealm.invitationLinks.some((link) =>
      link.entries.some((entry) => entry.tournamentClassId === tournamentClassId)
    );

    this.isInitialized = true;
  }

  protected save(): void {
    this.nameInvalid = false;

    const trimmedName = this.name.trim();
    if (trimmedName.length === 0) {
      this.nameInvalid = true;
      return;
    }

    if (this.maxTeamCount !== undefined && this.maxTeamCount < 2) {
      return;
    }

    if (trimmedName === this.initialName.trim() && this.maxTeamCount === this.initialMaxTeamCount) {
      this.modal.dismiss();
    } else {
      this.modal.close({
        name: trimmedName,
        maxTeamCount: this.maxTeamCount
      });
    }
  }

  protected limitMaxTeamCountChanged(limitMaxTeamCount: boolean): void {
    this.maxTeamCount = limitMaxTeamCount ? 10 : undefined;
  }
}
