import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ImageDto, ImageType, InvitationLinkDto, PlanningRealmDto, TournamentClassDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';

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

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly Actions = Actions;
  protected readonly ImageType = ImageType;

  constructor(protected readonly authorizationService: AuthorizationService) {}

  protected findTournamentClassById(id: number): TournamentClassDto {
    const tournamentClass = this.planningRealm.tournamentClasses.find((x) => x.id === id);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${id} does not exist in planning realm.`);
    }

    return tournamentClass;
  }

  protected setImage(whichImage: 'primaryLogo' | 'secondaryLogo', image?: ImageDto): void {
    if (this.invitationLink[whichImage]?.id === image?.id) {
      return;
    }

    this.updateInvitationLink((invitationLink) => (invitationLink[whichImage] = image ?? null));
  }

  protected onImageDeleted(imageId: string): void {
    // If an image is deleted, make sure to remove it from all invitation links that refer to it
    this.updatePlanningRealm((planningRealm) => {
      for (const invitationLink of planningRealm.invitationLinks) {
        if (invitationLink.primaryLogo?.id === imageId) {
          invitationLink.primaryLogo = null;
        }
        if (invitationLink.secondaryLogo?.id === imageId) {
          invitationLink.secondaryLogo = null;
        }
      }
      return true;
    });
  }

  protected deleteInvitationLink(): void {
    this.updatePlanningRealm((planningRealm) => {
      const index = planningRealm.invitationLinks.findIndex((x) => x.id === this.invitationLink.id);

      if (index === -1) {
        return false;
      }

      planningRealm.invitationLinks.splice(index, 1);
      return true;
    });
  }

  private updateInvitationLink(updateFunc: (invitationLink: InvitationLinkDto) => void): void {
    this.updatePlanningRealm((planningRealm) => {
      const invitationLink = planningRealm.invitationLinks.find((x) => x.id === this.invitationLink.id);

      if (!invitationLink) {
        return false;
      }

      updateFunc(invitationLink);
      return true;
    });
  }
}
