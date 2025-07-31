import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { ImageDto, ImageType, InvitationLinkDto, InvitationLinkEntryDto, PlanningRealmDto, TournamentClassDto } from '../../../api';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

type EditPropertiesDialogState = {
  colorCode: string;
  contactEmail: string;
  contactPerson: string;
  contactTelephone: string;
  description: string;
  externalLinks: { name: string; url: string }[];
  name: string;
  title: string;
  hasValidUntilDate: boolean;
  validUntil: string;
};

type EditEntryDialogState = {
  // Cannot be changed using dialog but is required for saving changes
  tournamentClassId: number;

  allowNewRegistrations: boolean;
  limitTeamsPerRegistration: boolean;
  maxTeamsPerRegistration: number;
};

@Component({
  standalone: false,
  selector: 'tp-invitation-link-tile',
  templateUrl: './invitation-link-tile.component.html'
})
export class InvitationLinkTileComponent {
  private _planningRealm!: PlanningRealmDto;
  private _invitationLink!: InvitationLinkDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly Actions = Actions;
  protected readonly ImageType = ImageType;

  protected tournamentClassesToAdd: TournamentClassDto[] = [];
  protected editPropertiesDialogState?: EditPropertiesDialogState;
  protected editEntryDialogState?: EditEntryDialogState;

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly modalService: NgbModal
  ) {}

  @Input()
  public set invitationLink(value: InvitationLinkDto) {
    this._invitationLink = value;
    this.determineTournamentClassesToAdd();
  }

  @Input()
  public set planningRealm(value: PlanningRealmDto) {
    this._planningRealm = value;
    this.determineTournamentClassesToAdd();
  }

  public get invitationLink(): InvitationLinkDto {
    return this._invitationLink;
  }

  public get planningRealm(): PlanningRealmDto {
    return this._planningRealm;
  }

  protected findTournamentClassById(id: number): TournamentClassDto {
    const tournamentClass = this.planningRealm.tournamentClasses.find((x) => x.id === id);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${id} does not exist in planning realm.`);
    }

    return tournamentClass;
  }

  protected showEditPropertiesDialog(template: TemplateRef<unknown>): void {
    this.editPropertiesDialogState = {
      colorCode: this._invitationLink.colorCode,
      name: this._invitationLink.name,
      title: this._invitationLink.title ?? '',
      description: this._invitationLink.description ?? '',
      contactEmail: this._invitationLink.contactEmail ?? '',
      contactPerson: this._invitationLink.contactPerson ?? '',
      contactTelephone: this._invitationLink.contactTelephone ?? '',
      externalLinks: this._invitationLink.externalLinks,
      hasValidUntilDate: !!this._invitationLink.validUntil,
      validUntil: this._invitationLink.validUntil ?? ''
    };

    const ref = this.modalService.open(template, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    ref.closed.subscribe({
      next: () => {
        this.updateInvitationLink((invitationLink) => {
          if (!this.editPropertiesDialogState || this.editPropertiesDialogState.name.trim().length === 0) {
            return false;
          }

          const toNullIfEmpty = (input: string): string | null => {
            input = input.trim();
            return input.length === 0 ? null : input;
          };

          invitationLink.name = this.editPropertiesDialogState.name;
          invitationLink.colorCode = this.editPropertiesDialogState.colorCode;

          invitationLink.title = toNullIfEmpty(this.editPropertiesDialogState.title);
          invitationLink.description = toNullIfEmpty(this.editPropertiesDialogState.description);
          invitationLink.contactEmail = toNullIfEmpty(this.editPropertiesDialogState.contactEmail);
          invitationLink.contactPerson = toNullIfEmpty(this.editPropertiesDialogState.contactPerson);
          invitationLink.contactTelephone = toNullIfEmpty(this.editPropertiesDialogState.contactTelephone);

          invitationLink.externalLinks = this.editPropertiesDialogState.externalLinks
            .filter((ext) => ext.name.trim().length > 0 && ext.url.trim().length > 0)
            .map((ext) => ({ ...ext }));

          return true;
        });
      }
    });
  }

  protected showEditEntryDialog(entry: InvitationLinkEntryDto, template: TemplateRef<unknown>): void {
    this.editEntryDialogState = {
      tournamentClassId: entry.tournamentClassId,
      allowNewRegistrations: entry.allowNewRegistrations,
      limitTeamsPerRegistration: !!entry.maxTeamsPerRegistration,
      maxTeamsPerRegistration: entry.maxTeamsPerRegistration ?? 0
    };

    const ref = this.modalService.open(template, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    ref.closed.subscribe({
      next: () => {
        this.updateInvitationLink((invitationLink) => {
          if (!this.editEntryDialogState) {
            return false;
          }

          const classId = this.editEntryDialogState.tournamentClassId;
          const entry = invitationLink.entries.find((x) => x.tournamentClassId === classId);

          if (!entry) {
            return false;
          }

          entry.allowNewRegistrations = this.editEntryDialogState.allowNewRegistrations;
          entry.maxTeamsPerRegistration = this.editEntryDialogState.limitTeamsPerRegistration
            ? this.editEntryDialogState.maxTeamsPerRegistration
            : null;

          return true;
        });
      }
    });
  }

  protected setImage(whichImage: 'primaryLogo' | 'secondaryLogo', image?: ImageDto): void {
    if (this.invitationLink[whichImage]?.id === image?.id) {
      return;
    }

    this.updateInvitationLink((invitationLink) => {
      invitationLink[whichImage] = image ?? null;
      return true;
    });
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

  protected addTournamentClass(id: number): void {
    this.updateInvitationLink((invitationLink) => {
      invitationLink.entries.push({
        tournamentClassId: id,
        allowNewRegistrations: true,
        maxTeamsPerRegistration: null,
        numberOfTeams: -1 // This is displayed in the HTML as a '?'
      });

      return true;
    });
    this.determineTournamentClassesToAdd();
  }

  protected removeTournamentClass(id: number): void {
    this.updateInvitationLink((invitationLink) => {
      const index = invitationLink.entries.findIndex((x) => x.tournamentClassId === id);

      if (index === -1) {
        return false;
      }

      invitationLink.entries.splice(index, 1);
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

  private updateInvitationLink(updateFunc: (invitationLink: InvitationLinkDto) => boolean): void {
    this.updatePlanningRealm((planningRealm) => {
      const invitationLink = planningRealm.invitationLinks.find((x) => x.id === this.invitationLink.id);

      if (!invitationLink) {
        return false;
      }

      return updateFunc(invitationLink);
    });
  }

  private determineTournamentClassesToAdd(): void {
    if (this.planningRealm && this.invitationLink) {
      this.tournamentClassesToAdd = this.planningRealm.tournamentClasses.filter(
        (tc) => tc.id > 0 && !this.invitationLink.entries.some((entry) => entry.tournamentClassId === tc.id)
      );
    } else {
      this.tournamentClassesToAdd = [];
    }
  }
}
