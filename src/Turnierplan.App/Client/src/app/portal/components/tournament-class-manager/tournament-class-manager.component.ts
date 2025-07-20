import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TournamentClassDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';

@Component({
  standalone: false,
  selector: 'tp-tournament-classes-manager',
  templateUrl: './tournament-class-manager.component.html'
})
export class TournamentClassManagerComponent {
  @Input()
  public planningRealmId!: string;

  @Input()
  public tournamentClasses: TournamentClassDto[] = [];

  @Output()
  public deleteClick = new EventEmitter<number>();

  protected readonly Actions = Actions;
  protected currentlyUpdatingId?: number;

  constructor(protected readonly authorizationService: AuthorizationService) {}
}
