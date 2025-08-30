import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { Actions } from '../../../generated/actions';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { SmallSpinnerComponent } from '../../../shared/components/small-spinner/small-spinner.component';
import { NgClass, AsyncPipe } from '@angular/common';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';

export interface TeamView {
  id: number;
  name: string;
  outOfCompetition: boolean;
  entryFeePaidAt?: Date;
  groupId?: number;
  priority?: number;
  hasTeamLink: boolean;
  // IDEA: The properties below should probably be extracted from this interface (see MatchView)
  showLoadingIndicator: { name: boolean; priority: boolean; entryFee: boolean; outOfCompetition: boolean };
}

@Component({
    selector: 'tp-team-list',
    templateUrl: './team-list.component.html',
    imports: [TranslateDirective, TooltipIconComponent, RenameButtonComponent, SmallSpinnerComponent, NgClass, NgbPopover, ActionButtonComponent, AsyncPipe, TranslatePipe, TranslateDatePipe]
})
export class TeamListComponent {
  protected readonly Actions = Actions;

  @Input()
  public tournamentId: string = '';

  @Input()
  public teams: TeamView[] = [];

  @Output()
  public teamRename = new EventEmitter<{ teamId: number; name: string }>();

  @Output()
  public teamSetPriority = new EventEmitter<{ teamId: number; groupId: number; priority: number }>();

  @Output()
  public teamSetEntryFeePaid = new EventEmitter<{ teamId: number; entryFeePaid: boolean }>();

  @Output()
  public teamSetOutOfCompetition = new EventEmitter<{ teamId: number; outOfCompetition: boolean }>();

  constructor(protected readonly authorizationService: AuthorizationService) {}

  protected get isUpdatingAnyTeam(): boolean {
    return this.teams.some(
      (x) =>
        x.showLoadingIndicator.name ||
        x.showLoadingIndicator.priority ||
        x.showLoadingIndicator.entryFee ||
        x.showLoadingIndicator.outOfCompetition
    );
  }

  protected renameTeam(teamId: number, name: string): void {
    if (this.isUpdatingAnyTeam) {
      return;
    }

    this.teamRename.emit({ teamId: teamId, name: name });
  }

  protected setTeamPriority(teamId: number, direction: number): void {
    if (this.isUpdatingAnyTeam) {
      return;
    }

    const team = this.teams.find((x) => x.id === teamId);

    if (!team || team.groupId === undefined || team.priority === undefined) {
      return;
    }

    this.teamSetPriority.emit({ teamId: teamId, groupId: team.groupId, priority: team.priority + direction });
  }

  protected setTeamEntryFeePaid(teamId: number, entryFeePaid: boolean): void {
    if (this.isUpdatingAnyTeam) {
      return;
    }

    this.teamSetEntryFeePaid.emit({ teamId: teamId, entryFeePaid: entryFeePaid });
  }

  protected toggleTeamOutOfCompetition(teamId: number): void {
    if (this.isUpdatingAnyTeam) {
      return;
    }

    const team = this.teams.find((x) => x.id === teamId);

    if (!team) {
      return;
    }

    this.teamSetOutOfCompetition.emit({ teamId: teamId, outOfCompetition: !team.outOfCompetition });
  }
}
