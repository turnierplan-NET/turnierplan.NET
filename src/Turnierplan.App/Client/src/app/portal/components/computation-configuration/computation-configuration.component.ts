import { Component } from '@angular/core';
import { NgbActiveModal, NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

import { ComputationConfigurationDto } from '../../../api';

import { availableComparisonModeOptions, ComparisonModeOption } from './computation-configuration.component-data';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
  templateUrl: './computation-configuration.component.html',
  imports: [TranslateDirective, FormsModule, NgClass, NgbTooltip, SmallSpinnerComponent, ActionButtonComponent, TranslatePipe]
})
export class ComputationConfigurationComponent {
  public save$ = new Subject<ComputationConfigurationDto>();

  protected isSaving = false;

  protected matchWonPoints: number = 0;
  protected matchDrawnPoints: number = 0;
  protected matchLostPoints: number = 0;
  protected higherScoreLoses: boolean = false;

  protected standardComparisonModeOptions: ComparisonModeOption[] = [];
  protected nonStandardComparisonModeOptions: ComparisonModeOption[] = [];
  protected comparisonModeOptionId: string = '';

  protected matchWonPointsInvalid: boolean = false;
  protected matchDrawnPointsInvalid: boolean = false;
  protected matchLostPointsInvalid: boolean = false;
  protected comparisonModesInvalid: boolean = false;

  constructor(protected readonly modal: NgbActiveModal) {
    this.standardComparisonModeOptions = availableComparisonModeOptions.filter((x) => x.isStandard);
    this.nonStandardComparisonModeOptions = availableComparisonModeOptions.filter((x) => !x.isStandard);
  }

  public initialize(computationConfiguration: ComputationConfigurationDto): void {
    this.matchWonPoints = computationConfiguration.matchWonPoints;
    this.matchDrawnPoints = computationConfiguration.matchDrawnPoints;
    this.matchLostPoints = computationConfiguration.matchLostPoints;
    this.higherScoreLoses = computationConfiguration.higherScoreLoses;

    this.comparisonModeOptionId = '';
    for (const option of availableComparisonModeOptions) {
      if (
        computationConfiguration.comparisonModes.length === option.modes.length &&
        computationConfiguration.comparisonModes.every((x, i) => x === option.modes[i])
      ) {
        this.comparisonModeOptionId = `${option.id}`;
        break;
      }
    }
  }

  protected saveClicked(): void {
    const comparisonModes = availableComparisonModeOptions.find((x) => x.id === +this.comparisonModeOptionId)?.modes;

    this.matchWonPointsInvalid = this.matchWonPoints < 0 || this.matchWonPoints > 100;
    this.matchDrawnPointsInvalid = this.matchDrawnPoints < 0 || this.matchDrawnPoints > 100;
    this.matchLostPointsInvalid = this.matchLostPoints < 0 || this.matchLostPoints > 100;
    this.comparisonModesInvalid = !comparisonModes || comparisonModes.length === 0;

    if (this.matchWonPointsInvalid || this.matchDrawnPointsInvalid || this.matchLostPointsInvalid || this.comparisonModesInvalid) {
      return;
    }

    this.isSaving = true;
    this.save$.next({
      matchWonPoints: this.matchWonPoints,
      matchDrawnPoints: this.matchDrawnPoints,
      matchLostPoints: this.matchLostPoints,
      higherScoreLoses: this.higherScoreLoses,
      comparisonModes: [...(comparisonModes ?? [])]
    });
    this.save$.complete();
  }
}
