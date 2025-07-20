import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PlanningRealmDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TournamentClassDialogComponent } from '../tournament-class-dialog/tournament-class-dialog.component';
import { TournamentClassesService } from '../../../api/services/tournament-classes.service';
import { switchMap, tap } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  standalone: false,
  selector: 'tp-tournament-classes-manager',
  templateUrl: './tournament-class-manager.component.html'
})
export class TournamentClassManagerComponent {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly Actions = Actions;
  protected currentlyUpdatingId?: number;

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly tournamentClassService: TournamentClassesService,
    private readonly modalService: NgbModal
  ) {}

  protected editTournamentClass(id: number): void {
    const ref = this.modalService.open(TournamentClassDialogComponent, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    (ref.componentInstance as TournamentClassDialogComponent).init(this.planningRealm, id);

    ref.closed
      .pipe(
        tap(() => (this.currentlyUpdatingId = id)),
        switchMap((result) =>
          this.tournamentClassService
            .updateTournamentClass({
              planningRealmId: this.planningRealm.id,
              id: id,
              body: { name: result.name, maxTeamCount: result.maxTeamCount }
            })
            .pipe(map(() => result))
        )
      )
      .subscribe({
        next: (result) => {
          this.currentlyUpdatingId = undefined;

          const tournamentClass = this.planningRealm.tournamentClasses.find((x) => x.id === id);

          if (tournamentClass) {
            tournamentClass.name = result.name;
            tournamentClass.maxTeamCount = result.maxTeamCount;
          }
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected deleteTournamentClass(id: number): void {
    // TODO: Implement delete logic
  }
}
