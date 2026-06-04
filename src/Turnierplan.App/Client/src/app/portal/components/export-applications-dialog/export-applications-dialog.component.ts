import { Component } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { exportApplications } from '../../../api/fn/applications/export-applications';

@Component({
  selector: 'tp-export-applications-dialog',
  imports: [TranslateDirective, ActionButtonComponent, SmallSpinnerComponent],
  templateUrl: './export-applications-dialog.component.html'
})
export class ExportApplicationsDialogComponent {
  protected isDownloading = false;
  private planningRealmId?: string;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly turnierplanApi: TurnierplanApi
  ) {}

  public initialize(planningRealmId: string) {
    this.planningRealmId = planningRealmId;
  }

  protected exportApplications(): void {
    if (!this.planningRealmId) {
      return;
    }

    this.isDownloading = true;

    this.turnierplanApi.invoke(exportApplications, { planningRealmId: this.planningRealmId }).subscribe({
      next: (result) => {
        const a = document.createElement('a');
        a.href = URL.createObjectURL(new Blob([result]));
        a.download = 'asdf.csv'; // TODO: Add proper translated file name
        a.click();

        this.modal.close();
      }
    });
  }
}
