import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ImageType, InvitationLinkDto, PlanningRealmDto, InvitationLinkEntryDto } from '../../../api';

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

  protected findEntryForClass(id: number): InvitationLinkEntryDto | undefined {
    return this.invitationLink.entries.find((x) => x.tournamentClassId === id);
  }

  protected setImage(which: 'primaryLogo' | 'secondaryLogo', imageId?: string): void {
    // TODO: Implement
  }
}
