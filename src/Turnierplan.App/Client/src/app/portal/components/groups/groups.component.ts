import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Actions } from '../../../generated/actions';
import { NgClass } from '@angular/common';
import { SmallSpinnerComponent } from '../../../shared/components/small-spinner/small-spinner.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed/is-action-allowed.directive';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { TranslateDirective } from '@ngx-translate/core';

export interface GroupView {
  id: number;
  displayName: string;
  hasCustomDisplayName: boolean;
  alphabeticalId: string;
  teams: GroupTeamView[];
  // IDEA: The properties below should probably be extracted from this interface (see MatchView)
  showLoadingIndicator: boolean;
}

export interface GroupTeamView {
  id: number;
  position: number;
  name: string;
  matches: number;
  won: number;
  drawn: number;
  lost: number;
  score: string;
  scoreDiff: number;
  points: number;
}

@Component({
    selector: 'tp-groups',
    templateUrl: './groups.component.html',
    imports: [NgClass, SmallSpinnerComponent, IsActionAllowedDirective, RenameButtonComponent, TranslateDirective]
})
export class GroupsComponent {
  protected readonly Actions = Actions;

  @Input()
  public tournamentId: string = '';

  @Input()
  public groups: GroupView[] = [];

  @Output()
  public groupRename = new EventEmitter<{ groupId: number; name?: string }>();

  protected get isUpdatingAnyGroup(): boolean {
    return this.groups.some((x) => x.showLoadingIndicator);
  }

  protected renameGroup(groupId: number, name?: string): void {
    if (this.isUpdatingAnyGroup) {
      return;
    }

    this.groupRename.emit({ groupId: groupId, name: name });
  }
}
