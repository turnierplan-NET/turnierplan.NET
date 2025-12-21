import { Component, inject } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { CreateRankingOverwriteEndpointRequest } from '../../../api/models/create-ranking-overwrite-endpoint-request';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { NgClass } from '@angular/common';

@Component({
  selector: 'tp-new-ranking-overwrite-dialog',
  imports: [TranslateDirective, ActionButtonComponent, ReactiveFormsModule, TooltipIconComponent, NgClass],
  templateUrl: './new-ranking-overwrite-dialog.component.html'
})
export class NewRankingOverwriteDialogComponent {
  protected readonly form = new FormGroup({
    placementRank: new FormControl(1, { nonNullable: true, validators: [Validators.min(1)] }),
    hideRanking: new FormControl(false, { nonNullable: true, validators: [Validators.required] }),
    assignTeamId: new FormControl(0)
  });

  protected readonly modal = inject(NgbActiveModal);
  protected _teams: { id: number; name: string }[] = [];

  public set teams(value: { id: number; name: string }[]) {
    this._teams = [...value];
    this._teams.sort((a, b) => a.name.localeCompare(b.name));
  }

  protected submit(): void {
    // TODO: Check if allowed based on the existing ranking overwrites

    if (this.form.invalid) {
      return;
    }

    this.modal.close({
      placementRank: this.form.value.placementRank,
      hideRanking: this.form.value.hideRanking,
      assignTeamId: this.form.value.hideRanking || this.form.value.assignTeamId === 0 ? undefined : this.form.value.assignTeamId
    } as CreateRankingOverwriteEndpointRequest);
  }
}
