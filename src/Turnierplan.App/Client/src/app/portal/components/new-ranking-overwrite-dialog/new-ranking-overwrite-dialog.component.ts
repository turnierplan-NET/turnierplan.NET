import { Component, inject } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { CreateRankingOverwriteEndpointRequest } from '../../../api/models/create-ranking-overwrite-endpoint-request';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TooltipIconComponent } from '../tooltip-icon/tooltip-icon.component';
import { NgClass } from '@angular/common';
import { RankingOverwriteDto } from '../../../api/models/ranking-overwrite-dto';
import { AlertComponent } from '../alert/alert.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'tp-new-ranking-overwrite-dialog',
  imports: [TranslateDirective, ActionButtonComponent, ReactiveFormsModule, TooltipIconComponent, NgClass, AlertComponent],
  templateUrl: './new-ranking-overwrite-dialog.component.html'
})
export class NewRankingOverwriteDialogComponent {
  protected readonly form = new FormGroup({
    placementRank: new FormControl(1, { nonNullable: true, validators: [Validators.min(1)] }),
    hideRanking: new FormControl(false, { nonNullable: true, validators: [Validators.required] }),
    assignTeamId: new FormControl('0', { nonNullable: true })
  });

  protected readonly modal = inject(NgbActiveModal);
  protected hasConflictWithExisting = false;
  protected _teams: { id: number; name: string }[] = [];

  private _existingOverwrites: RankingOverwriteDto[] = [];

  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed()).subscribe(() => (this.hasConflictWithExisting = false));
  }

  public set teams(value: { id: number; name: string }[]) {
    this._teams = [...value];
    this._teams.sort((a, b) => a.name.localeCompare(b.name));
  }

  public set existingOverwrites(value: RankingOverwriteDto[]) {
    this._existingOverwrites = value;
  }

  protected submit(): void {
    if (this.form.invalid) {
      return;
    }

    const request = {
      placementRank: this.form.value.placementRank,
      hideRanking: this.form.value.hideRanking,
      assignTeamId: this.form.value.hideRanking || this.form.value.assignTeamId === '0' ? undefined : +this.form.getRawValue().assignTeamId
    } as CreateRankingOverwriteEndpointRequest;

    // The code below "mirrors" the precondition logic in the AddRankingOverwrite() backend method
    if (request.hideRanking) {
      if (this._existingOverwrites.some((existing) => existing.placementRank === request.placementRank)) {
        this.hasConflictWithExisting = true;
        return;
      }
    } else {
      console.log(request);
      if (
        this._existingOverwrites.some(
          (existing) =>
            existing.placementRank === request.placementRank && (existing.hideRanking || existing.assignTeamId === request.assignTeamId)
        )
      ) {
        this.hasConflictWithExisting = true;
        return;
      }
    }

    this.modal.close(request);
  }
}
