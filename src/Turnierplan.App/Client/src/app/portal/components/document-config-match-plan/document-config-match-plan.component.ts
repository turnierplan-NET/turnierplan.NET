import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { MatchPlanDateFormat, MatchPlanDocumentConfiguration, MatchPlanOutcomes } from '../../../api';
import { CURRENT_CONFIGURATION, DocumentConfigComponent } from '../document-config-frame/document-config-frame.component';

@Component({
  standalone: false,
  templateUrl: './document-config-match-plan.component.html'
})
export class DocumentConfigMatchPlanComponent extends DocumentConfigComponent<MatchPlanDocumentConfiguration> implements OnInit, OnDestroy {
  public submit = new Subject<void>();

  protected form: FormGroup;
  protected matchPlanDateFormats = Object.keys(MatchPlanDateFormat);
  protected matchPlanOutcomeTypes = Object.keys(MatchPlanOutcomes);

  private readonly destroyed$ = new Subject<void>();

  constructor(formBuilder: FormBuilder, @Inject(CURRENT_CONFIGURATION) config: MatchPlanDocumentConfiguration) {
    super();

    this.form = formBuilder.group({
      organizerNameOverride: [config.organizerNameOverride ?? ''],
      tournamentNameOverride: [config.tournamentNameOverride ?? ''],
      venueOverride: [config.venueOverride ?? ''],
      dateFormat: [config.dateFormat],
      outcomes: [config.outcomes],
      includeQrCode: [config.includeQrCode],
      includeRankingTable: [config.includeRankingTable]
    });

    if (config.outcomes === MatchPlanOutcomes.HideOutcomeStructures) {
      this.includeRankingTable.disable();
    }
  }

  protected get organizerNameOverride(): AbstractControl {
    return this.form.get('organizerNameOverride')!;
  }

  protected get tournamentNameOverride(): AbstractControl {
    return this.form.get('tournamentNameOverride')!;
  }

  protected get venueOverride(): AbstractControl {
    return this.form.get('venueOverride')!;
  }

  protected get outcomes(): AbstractControl {
    return this.form.get('outcomes')!;
  }

  protected get includeRankingTable(): AbstractControl {
    return this.form.get('includeRankingTable')!;
  }

  public getConfig(): MatchPlanDocumentConfiguration {
    return this.form.value as MatchPlanDocumentConfiguration;
  }

  public isValid(): boolean {
    return this.form.valid;
  }

  public ngOnInit(): void {
    this.outcomes.valueChanges.pipe(takeUntil(this.destroyed$)).subscribe({
      next: (value) => {
        if (value === 'HideOutcomeStructures') {
          this.includeRankingTable.disable();
          this.includeRankingTable.setValue(false);
        } else {
          this.includeRankingTable.enable();
        }
      }
    });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();

    this.submit.complete();
  }
}
