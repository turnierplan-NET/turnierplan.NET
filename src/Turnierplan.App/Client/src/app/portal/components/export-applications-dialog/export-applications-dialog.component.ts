import { Component } from '@angular/core';
import { TranslateDirective, TranslateService } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { exportApplications } from '../../../api/fn/applications/export-applications';
import { TournamentPlannerDto } from '../../../api/models/tournament-planner-dto';
import { makeSafeFileName } from '../../helpers/file-name';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'tp-export-applications-dialog',
  imports: [TranslateDirective, ActionButtonComponent, SmallSpinnerComponent, ReactiveFormsModule],
  templateUrl: './export-applications-dialog.component.html'
})
export class ExportApplicationsDialogComponent {
  protected readonly form = new FormGroup({
    includeApplicationTeams: new FormControl(false, { nonNullable: true })
  });

  protected isDownloading = false;
  private tournamentPlanner?: TournamentPlannerDto;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly turnierplanApi: TurnierplanApi,
    private readonly translateService: TranslateService
  ) {}

  public initialize(tournamentPlanner: TournamentPlannerDto) {
    this.tournamentPlanner = tournamentPlanner;
  }

  protected exportApplications(): void {
    if (!this.tournamentPlanner) {
      return;
    }

    const fileName = `${makeSafeFileName(
      this.translateService.instant('Portal.ViewPlanningRealm.ExportApplications.FileName', {
        tournamentPlannerName: this.tournamentPlanner?.name
      }) as string
    )}.csv`;

    this.form.disable();
    this.isDownloading = true;

    this.turnierplanApi
      .invoke(exportApplications, {
        tournamentPlannerId: this.tournamentPlanner.id,
        languageCode: this.translateService.getCurrentLang(),
        includeApplicationTeams: this.form.getRawValue().includeApplicationTeams
      })
      .subscribe({
        next: (result) => {
          const a = document.createElement('a');
          a.href = URL.createObjectURL(new Blob([result]));
          a.download = fileName;
          a.click();

          this.modal.close();
        },
        error: (error) => {
          this.modal.dismiss({ isApiError: true, apiError: error as unknown });
        }
      });
  }
}
