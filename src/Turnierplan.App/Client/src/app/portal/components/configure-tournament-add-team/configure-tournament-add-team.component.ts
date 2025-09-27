import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { FormsModule } from '@angular/forms';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgbActiveModal, NgbNav, NgbNavContent, NgbNavItem, NgbNavLinkButton, NgbNavOutlet } from '@ng-bootstrap/ng-bootstrap';
import { LocalStorageService } from '../../services/local-storage.service';
import { NgClass } from '@angular/common';
import { TemporaryTeam } from '../../pages/configure-tournament/configure-tournament.component';
import { SelectApplicationTeamComponent, SelectApplicationTeamResult } from '../select-application-team/select-application-team.component';
import { PublicId } from '../../../api/models/public-id';
import { E2eDirective } from '../../../core/directives/e2e.directive';

enum AddTeamMode {
  NewTeam = 'NewTeam',
  ImportTeam = 'ImportTeam'
}

@Component({
  imports: [
    ActionButtonComponent,
    FormsModule,
    TranslateDirective,
    NgbNav,
    NgbNavItem,
    NgbNavLinkButton,
    NgbNavContent,
    NgbNavOutlet,
    NgClass,
    TranslatePipe,
    SelectApplicationTeamComponent,
    E2eDirective
  ],
  templateUrl: './configure-tournament-add-team.component.html'
})
export class ConfigureTournamentAddTeamComponent implements AfterViewInit {
  protected readonly AddTeamMode = AddTeamMode;

  @ViewChild('addTeamNameInput')
  protected addTeamNameInput!: ElementRef<HTMLInputElement>;

  protected organizationId?: PublicId;
  protected usedApplicationTeamIds: number[] = [];
  protected currentMode: AddTeamMode = AddTeamMode.NewTeam;
  protected addTeamName: string = '';
  protected importTeamSelected?: SelectApplicationTeamResult;
  protected confirmAttempted = false;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly localStorageService: LocalStorageService
  ) {
    const value = localStorageService.getAddTeamDialogMode();
    this.currentMode = value === AddTeamMode.ImportTeam ? AddTeamMode.ImportTeam : AddTeamMode.NewTeam;
  }

  protected get addTeamNameInvalid(): boolean {
    return this.addTeamName.trim().length === 0;
  }

  public ngAfterViewInit(): void {
    if (this.currentMode === AddTeamMode.NewTeam) {
      this.addTeamNameInput.nativeElement.focus();
      setTimeout(() => this.addTeamNameInput.nativeElement.select(), 20);
    }
  }

  public init(organizationId: PublicId, usedApplicationTeamIds: number[]): void {
    this.organizationId = organizationId;
    this.usedApplicationTeamIds = usedApplicationTeamIds;
  }

  protected currentModeChanged(): void {
    this.localStorageService.setAddTeamDialogMode(this.currentMode);

    if (this.currentMode === AddTeamMode.ImportTeam) {
      this.importTeamSelected = undefined;
    }
  }

  protected confirm(): void {
    this.confirmAttempted = true;

    switch (this.currentMode) {
      case AddTeamMode.NewTeam: {
        const trimmed = this.addTeamName.trim();
        if (trimmed.length > 0) {
          this.modal.close([
            {
              id: undefined,
              name: this.addTeamName
            }
          ] as TemporaryTeam[]);
        }
        break;
      }
      case AddTeamMode.ImportTeam: {
        if (this.importTeamSelected) {
          this.modal.close(
            this.importTeamSelected.map(
              (team) =>
                ({
                  id: undefined,
                  name: team.name,
                  teamLink: {
                    planningRealmId: team.planningRealmId,
                    planningRealmName: team.planningRealmName,
                    tournamentClassName: team.tournamentClassName,
                    applicationTeamId: team.applicationTeamId
                  }
                }) as TemporaryTeam
            )
          );
        }
        break;
      }
    }
  }
}
