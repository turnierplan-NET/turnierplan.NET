import { Component, Input } from '@angular/core';
import { InvitationLinkDto } from '../../../api';

@Component({
  standalone: false,
  selector: 'tp-invitation-links-manager',
  templateUrl: './invitation-link-manager.component.html'
})
export class InvitationLinkManagerComponent {
  @Input()
  public planningRealmId!: string;

  @Input()
  public invitationLinks: InvitationLinkDto[] = [];
}
