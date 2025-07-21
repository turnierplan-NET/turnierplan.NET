import { Component, Input } from '@angular/core';
import { InvitationLinkDto } from '../../../api';

@Component({
  standalone: false,
  selector: 'tp-invitation-link-tile',
  templateUrl: './invitation-link-tile.component.html'
})
export class InvitationLinkTileComponent {
  @Input()
  public planningRealmId!: string;

  @Input()
  public invitationLink!: InvitationLinkDto;
}
