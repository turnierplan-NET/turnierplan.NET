import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TournamentClassDto } from '../../../api';
import { max } from 'rxjs';

@Component({
  standalone: false,
  templateUrl: './tournament-class-dialog.component.html'
})
export class TournamentClassDialogComponent {
  protected isInitialized = false;
  protected initialName: string = '';
  protected initialMaxTeamCount?: number;
  protected nameInvalid = false;

  protected name: string = '';
  protected maxTeamCount?: number;

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(tournamentClass: TournamentClassDto): void {
    this.name = tournamentClass.name;
    this.maxTeamCount = tournamentClass.maxTeamCount ?? undefined;

    this.initialName = tournamentClass.name;
    this.initialMaxTeamCount = tournamentClass.maxTeamCount ?? undefined;
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
