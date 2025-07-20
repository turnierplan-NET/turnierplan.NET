import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TournamentClassDto } from '../../../api';
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
  public planningRealmId!: string;

  @Input()
  public tournamentClasses: TournamentClassDto[] = [];

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

    ref.closed
      .pipe(
        tap(() => (this.currentlyUpdatingId = id)),
        switchMap((result) =>
          this.tournamentClassService
            .updateTournamentClass({
              planningRealmId: this.planningRealmId,
              id: id,
              body: { name: result.name, maxTeamCount: result.maxTeamCount }
            })
            .pipe(map(() => result))
        )
      )
      .subscribe({
        next: (result) => {
          this.currentlyUpdatingId = undefined;

          tournamentClass.name = result.name;
          tournamentClass.maxTeamCount = result.maxTeamCount;
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }
}
