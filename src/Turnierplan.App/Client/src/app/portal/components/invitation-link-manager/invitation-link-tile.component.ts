import { Component, EventEmitter, Input, Output } from '@angular/core';
import {
  ImageType,
  InvitationLinkDto,
  PlanningRealmDto,
  InvitationLinkEntryDto,
  TournamentsService,
  TournamentClassDto
} from '../../../api';

@Component({
  standalone: false,
  selector: 'tp-invitation-link-tile',
  templateUrl: './invitation-link-tile.component.html'
})
export class InvitationLinkTileComponent {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Input()
  public invitationLink!: InvitationLinkDto;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  @Output()
  public deleteRequest = new EventEmitter<number>(); // TODO: Emit delete delete event necessary?

  protected readonly ImageType = ImageType;

  protected findTournamentClassById(id: number): TournamentClassDto {
    const tournamentClass = this.planningRealm.tournamentClasses.find((x) => x.id === id);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${id} does not exist in planning realm.`);
    }

    return tournamentClass;
  }

  protected setImage(which: 'primaryLogo' | 'secondaryLogo', imageId?: string): void {
    // TODO: Implement
  }
}
