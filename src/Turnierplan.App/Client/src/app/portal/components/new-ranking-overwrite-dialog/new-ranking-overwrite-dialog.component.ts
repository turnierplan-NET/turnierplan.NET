import { Component, inject } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { CreateRankingOverwriteEndpointRequest } from '../../../api/models/create-ranking-overwrite-endpoint-request';

@Component({
  selector: 'tp-new-ranking-overwrite-dialog',
  imports: [TranslateDirective, ActionButtonComponent],
  templateUrl: './new-ranking-overwrite-dialog.component.html'
})
export class NewRankingOverwriteDialogComponent {
  protected readonly modal = inject(NgbActiveModal);
  protected _teams: { id: number; name: string }[] = [];

  public set teams(value: { id: number; name: string }[]) {
    this._teams = value;
  }

  protected submit(): void {
    this.modal.close({
      placementRank: 2,
      hideRanking: true,
      assignTeamId: undefined
    } as CreateRankingOverwriteEndpointRequest);
  }
}
