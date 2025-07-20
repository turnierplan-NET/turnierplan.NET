import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TournamentClassDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TournamentClassDialogComponent } from '../tournament-class-dialog/tournament-class-dialog.component';

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

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly modalService: NgbModal
  ) {}

  protected editTournamentClass(id: number): void {
    const tournamentClass = this.tournamentClasses.find((x) => x.id === id);

    if (!tournamentClass) {
      return;
    }

    const ref = this.modalService.open(TournamentClassDialogComponent, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    (ref.componentInstance as TournamentClassDialogComponent).init(tournamentClass);

    ref.closed.subscribe({ next: (x) => console.log(x) });
  }
}
