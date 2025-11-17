import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { UpdatePlanningRealmFunc } from '../../pages/view-planning-realm/view-planning-realm.component';
import { NgbModal, NgbTooltip, NgbDropdown, NgbDropdownToggle, NgbDropdownMenu, NgbDropdownItem } from '@ng-bootstrap/ng-bootstrap';
import { FormArray, FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { formatDate, NgStyle, NgClass, AsyncPipe } from '@angular/common';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { ShareLinkComponent } from '../share-link/share-link.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { ImageWidgetComponent } from '../image-widget/image-widget.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { AlertComponent } from '../alert/alert.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { InvitationLinkDto } from '../../../api/models/invitation-link-dto';
import { ImageType } from '../../../api/models/image-type';
import { TournamentClassDto } from '../../../api/models/tournament-class-dto';
import { InvitationLinkEntryDto } from '../../../api/models/invitation-link-entry-dto';
import { ImageDto } from '../../../api/models/image-dto';

@Component({
  selector: 'tp-invitation-link-tile',
  templateUrl: './invitation-link-tile.component.html',
  imports: [
    NgStyle,
    TranslateDirective,
    TooltipIconComponent,
    ShareLinkComponent,
    NgbTooltip,
    NgClass,
    ActionButtonComponent,
    ImageWidgetComponent,
    IsActionAllowedDirective,
    DeleteButtonComponent,
    NgbDropdown,
    NgbDropdownToggle,
    NgbDropdownMenu,
    NgbDropdownItem,
    FormsModule,
    ReactiveFormsModule,
    AlertComponent,
    AsyncPipe,
    TranslatePipe,
    TranslateDatePipe
  ]
})
export class InvitationLinkTileComponent {
  private _planningRealm!: PlanningRealmDto;
  private _invitationLink!: InvitationLinkDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();

  protected readonly Actions = Actions;
  protected readonly ImageType = ImageType;
  protected invitationLinkExpired = false;

  protected tournamentClassesToAdd: TournamentClassDto[] = [];
  protected editPropertiesForm = new FormGroup({
    name: new FormControl<string>('', { nonNullable: true, validators: Validators.required }),
    colorCode: new FormControl<string>('', {
      nonNullable: true,
      validators: Validators.compose([Validators.required, Validators.pattern(/^#[0-9A-Fa-f]{6}$/)])
    }),
    title: new FormControl<string>(''),
    description: new FormControl<string>(''),
    contactPerson: new FormControl<string>(''),
    contactEmail: new FormControl<string>(''),
    contactTelephone: new FormControl<string>(''),
    externalLinks: new FormArray<FormGroup<{ name: FormControl<string>; url: FormControl<string> }>>([]),
    hasValidUntilDate: new FormControl<boolean>(false, { nonNullable: true }),
    validUntil: new FormControl<string>('', { nonNullable: true })
  });

  protected editEntryTournamentClassId: number = 0;
  protected editEntryForm = new FormGroup({
    allowNewRegistrations: new FormControl<boolean>(false, { nonNullable: true }),
    limitTeamsPerRegistration: new FormControl<boolean>(false, { nonNullable: true }),
    maxTeamsPerRegistration: new FormControl<number>(0, { nonNullable: true, validators: Validators.min(1) })
  });

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly modalService: NgbModal
  ) {}

  @Input()
  public set invitationLink(value: InvitationLinkDto) {
    this._invitationLink = value;
    this.determineExpired();
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

  protected get editPropertiesFormExternalLinks(): FormArray {
    return this.editPropertiesForm.get('externalLinks')! as FormArray;
  }

  protected findTournamentClassById(id: number): TournamentClassDto {
    const tournamentClass = this.planningRealm.tournamentClasses.find((x) => x.id === id);

    if (!tournamentClass) {
      throw new Error(`Tournament class id ${id} does not exist in planning realm.`);
    }

    return tournamentClass;
  }

  protected showEditPropertiesDialog(template: TemplateRef<unknown>): void {
    this.editPropertiesForm.patchValue({
      colorCode: `#${this._invitationLink.colorCode}`,
      name: this._invitationLink.name,
      title: this._invitationLink.title ?? '',
      description: this._invitationLink.description ?? '',
      contactPerson: this._invitationLink.contactPerson ?? '',
      contactEmail: this._invitationLink.contactEmail ?? '',
      contactTelephone: this._invitationLink.contactTelephone ?? '',
      externalLinks: [],
      hasValidUntilDate: this._invitationLink.validUntil !== null,
      validUntil: formatDate(this._invitationLink.validUntil ? this._invitationLink.validUntil : new Date(), 'yyyy-MM-ddTHH:mm', 'de')
    });

    this.editPropertiesFormExternalLinks.clear();
    for (const externalLink of this._invitationLink.externalLinks) {
      this.addExternalLinkToForm(externalLink.name, externalLink.url);
    }

    this.editPropertiesForm.markAsPristine({ onlySelf: false });

    const ref = this.modalService.open(template, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    ref.closed.subscribe({
      next: () => {
        this.updateInvitationLink((invitationLink) => {
          if (this.editPropertiesForm.invalid || this.editPropertiesForm.pristine) {
            return false;
          }

          const value = this.editPropertiesForm.getRawValue();

          const toUndefinedIfEmpty = (input: string | null): string | undefined => {
            if (input === null) {
              return undefined;
            }
            input = input.trim();
            return input.length === 0 ? undefined : input;
          };

          invitationLink.name = value.name;
          invitationLink.colorCode = value.colorCode.substring(1); // skip the '#' character

          invitationLink.title = toUndefinedIfEmpty(value.title);
          invitationLink.description = toUndefinedIfEmpty(value.description);
          invitationLink.contactPerson = toUndefinedIfEmpty(value.contactPerson);
          invitationLink.contactEmail = toUndefinedIfEmpty(value.contactEmail);
          invitationLink.contactTelephone = toUndefinedIfEmpty(value.contactTelephone);

          invitationLink.validUntil = value.hasValidUntilDate ? new Date(value.validUntil).toISOString() : undefined;

          invitationLink.externalLinks = value.externalLinks
            .filter((ext) => ext.name.trim().length > 0 && ext.url.trim().length > 0)
            .map((ext) => ({ ...ext }));

          return true;
        });

        this.determineExpired();
      }
    });
  }

  protected addExternalLinkToForm(name: string = '', url: string = ''): void {
    this.editPropertiesFormExternalLinks.push(
      new FormGroup({
        name: new FormControl<string>(name, { nonNullable: false, validators: Validators.required }),
        url: new FormControl<string>(url, {
          nonNullable: false,
          validators: Validators.compose([Validators.required, Validators.pattern(/^https:\/\/(?:[A-Za-z0-9-]+\.)+[a-z]+(?:\/.*)?$/)])
        })
      })
    );
  }

  protected showEditEntryDialog(entry: InvitationLinkEntryDto, template: TemplateRef<unknown>): void {
    this.editEntryTournamentClassId = entry.tournamentClassId;

    this.editEntryForm.patchValue({
      allowNewRegistrations: entry.allowNewRegistrations,
      limitTeamsPerRegistration: !!entry.maxTeamsPerRegistration,
      maxTeamsPerRegistration: entry.maxTeamsPerRegistration ?? 1
    });

    this.editEntryForm.markAsPristine({ onlySelf: false });

    const ref = this.modalService.open(template, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });

    ref.closed.subscribe({
      next: () => {
        this.updateInvitationLink((invitationLink) => {
          if (this.editEntryForm.invalid || this.editEntryForm.pristine) {
            return false;
          }

          const entry = invitationLink.entries.find((x) => x.tournamentClassId === this.editEntryTournamentClassId);

          if (!entry) {
            return false;
          }

          const value = this.editEntryForm.getRawValue();

          entry.allowNewRegistrations = value.allowNewRegistrations;
          entry.maxTeamsPerRegistration = value.limitTeamsPerRegistration ? value.maxTeamsPerRegistration : undefined;

          return true;
        });
      }
    });
  }

  protected setInvitationLinkActive(isActive: boolean): void {
    if (this.invitationLink.isActive === isActive) {
      return;
    }

    this.updateInvitationLink((invitationLink) => {
      invitationLink.isActive = isActive;
      return true;
    });
  }

  protected setImage(whichImage: 'primaryLogo' | 'secondaryLogo', image?: ImageDto): void {
    if (this.invitationLink[whichImage]?.id === image?.id) {
      return;
    }

    this.updateInvitationLink((invitationLink) => {
      invitationLink[whichImage] = image;
      return true;
    });
  }

  protected onImageDeleted(imageId: string): void {
    // If an image is deleted, make sure to remove it from all invitation links that refer to it
    this.updatePlanningRealm((planningRealm) => {
      for (const invitationLink of planningRealm.invitationLinks) {
        if (invitationLink.primaryLogo?.id === imageId) {
          invitationLink.primaryLogo = undefined;
        }
        if (invitationLink.secondaryLogo?.id === imageId) {
          invitationLink.secondaryLogo = undefined;
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
        maxTeamsPerRegistration: undefined,
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

    this.determineTournamentClassesToAdd();
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

  protected searchApplicationsClicked(tournamentClassId?: number): void {
    if (this.invitationLink.id < 0) {
      return;
    }

    this.filterRequested.emit({
      searchTerm: '',
      invitationLink: [this.invitationLink.id],
      tournamentClass: tournamentClassId === undefined ? [] : [tournamentClassId],
      label: []
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

  private determineExpired(): void {
    this.invitationLinkExpired = !!this._invitationLink.validUntil && new Date(this._invitationLink.validUntil) < new Date();
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
