import { Component, EventEmitter, Injector, Input, Output } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { switchMap } from 'rxjs';

import { HeaderLineContent, PresentationConfigurationDto, ResultsMode, TournamentsService } from '../../../api';
import { NotificationService } from '../../../core/services/notification.service';
import { TournamentSelectComponent } from '../tournament-select/tournament-select.component';

@Component({
  selector: 'tp-presentation-config-widget',
  templateUrl: './presentation-config-widget.component.html'
})
export class PresentationConfigWidgetComponent {
  @Input()
  public tournamentId!: string;

  @Input()
  public organizationId!: string;

  @Input()
  public organizationName!: string;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly ResultsMode = ResultsMode;
  protected readonly HeaderLineContent = HeaderLineContent;

  protected header1Content = HeaderLineContent.TournamentName;
  protected header2Content = HeaderLineContent.TournamentName;
  protected header1CustomContent = '';
  protected header2CustomContent = '';
  protected showResults: ResultsMode = ResultsMode.Default;
  protected showOrganizerLogo = true;
  protected showSponsorLogo = true;

  protected saveAttempted = false;
  protected hasUnsavedChanges = false;
  protected isSaving = false;

  constructor(
    private readonly tournamentService: TournamentsService,
    private readonly notificationService: NotificationService,
    private readonly modalService: NgbModal,
    private readonly injector: Injector
  ) {}

  protected get isHeader1CustomContentInvalid(): boolean {
    return (
      this.header1Content === HeaderLineContent.CustomValue &&
      (this.header1CustomContent.trim().length === 0 || this.header1CustomContent.length > 60)
    );
  }

  protected get isHeader2CustomContentInvalid(): boolean {
    return (
      this.header2Content === HeaderLineContent.CustomValue &&
      (this.header2CustomContent.trim().length === 0 || this.header2CustomContent.length > 60)
    );
  }

  @Input()
  public set config(value: PresentationConfigurationDto) {
    this.initialize(value);

    this.hasUnsavedChanges = false;
  }

  protected onChanged(): void {
    this.hasUnsavedChanges = true;
  }

  protected saveChanges(): void {
    if (!this.hasUnsavedChanges || this.isSaving) {
      return;
    }

    this.saveAttempted = true;

    if (this.isHeader1CustomContentInvalid || this.isHeader2CustomContentInvalid) {
      return;
    }

    this.isSaving = true;

    const data = {
      configuration: {
        header1: {
          content: this.header1Content,
          customContent: this.header1CustomContent.trim().length > 0 ? this.header1CustomContent : undefined
        },
        header2: {
          content: this.header2Content,
          customContent: this.header2CustomContent.trim().length > 0 ? this.header2CustomContent : undefined
        },
        showResults: this.showResults,
        showOrganizerLogo: this.showOrganizerLogo,
        showSponsorLogo: this.showSponsorLogo
      }
    };

    this.tournamentService.setTournamentPresentationConfiguration({ id: this.tournamentId, body: data }).subscribe({
      next: () => {
        this.hasUnsavedChanges = false;
        this.isSaving = false;
        this.saveAttempted = false;

        this.notificationService.showNotification(
          'success',
          'Portal.ViewTournament.Share.PresentationConfig.SuccessToast.Title',
          'Portal.ViewTournament.Share.PresentationConfig.SuccessToast.Message'
        );
      },
      error: (error) => {
        this.errorOccured.emit(error);
      }
    });
  }

  protected copyFromOtherTournament(): void {
    const ref = this.modalService.open(TournamentSelectComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg',
      injector: this.injector
    });

    const component = ref.componentInstance as TournamentSelectComponent;
    component.organization = {
      id: this.organizationId,
      name: this.organizationName
    };

    ref.closed.pipe(switchMap((id: string) => this.tournamentService.getTournamentPresentationConfiguration({ id: id }))).subscribe({
      next: (presentationConfiguration) => {
        this.initialize(presentationConfiguration);
        this.hasUnsavedChanges = true;
      },
      error: (error) => {
        this.errorOccured.emit(error);
      }
    });
  }

  private initialize(value: PresentationConfigurationDto): void {
    this.header1Content = value.header1.content;
    this.header2Content = value.header2.content;
    this.header1CustomContent = value.header1.customContent ?? '';
    this.header2CustomContent = value.header2.customContent ?? '';
    this.showResults = value.showResults;
    this.showOrganizerLogo = value.showOrganizerLogo;
    this.showSponsorLogo = value.showSponsorLogo;
  }
}
