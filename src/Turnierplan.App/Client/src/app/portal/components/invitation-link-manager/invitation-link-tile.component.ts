import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ImageType, InvitationLinkDto, PlanningRealmDto, TournamentClassDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';

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
  public reloadRequest = new EventEmitter<void>();

  @Output()
  public deleteRequest = new EventEmitter<number>(); // TODO: Emit delete delete event necessary?

  protected readonly ImageType = ImageType;
  protected isUpdatingImage = false;

  constructor(protected readonly authorizationService: AuthorizationService) {}

  protected findTournamentClassById(id: number): TournamentClassDto {
    const tournamentClass = this.planningRealm.tournamentClasses.find((x) => x.id === id);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${id} does not exist in planning realm.`);
    }

    return tournamentClass;
  }

  protected setImage(whichImage: 'primaryLogo' | 'secondaryLogo', imageId?: string): void {
    if (this.invitationLink[whichImage]?.id === imageId) {
      return;
    }

    this.isUpdatingImage = true;

    // TODO: Re-implement for new endpoint
    // const mappedTarget = {
    //   primaryLogo: SetInvitationLinkImageEndpointRequestTarget.PrimaryLogo,
    //   secondaryLogo: SetInvitationLinkImageEndpointRequestTarget.SecondaryLogo
    // }[whichImage];
    //
    // this.invitationLinkService
    //   .setInvitationLinkImage({
    //     id: this.invitationLink.id,
    //     planningRealmId: this.planningRealm.id,
    //     body: { imageId: imageId, target: mappedTarget }
    //   })
    //   .subscribe({
    //     next: () => {
    //       this.reloadRequest.emit();
    //     },
    //     error: (error) => {
    //       this.errorOccured.emit(error);
    //     }
    //   });
  }

  protected readonly Actions = Actions;
}
