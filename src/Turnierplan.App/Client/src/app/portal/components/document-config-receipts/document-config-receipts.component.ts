import { Component, Inject, OnDestroy } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';

import { ReceiptsDocumentConfiguration } from '../../../api';
import { CURRENT_CONFIGURATION, DocumentConfigComponent } from '../document-config-frame/document-config-frame.component';

@Component({
  standalone: false,
  templateUrl: './document-config-receipts.component.html'
})
export class DocumentConfigReceiptsComponent extends DocumentConfigComponent<ReceiptsDocumentConfiguration> implements OnDestroy {
  private static readonly maxNumberOfAmountEntries = 4;

  public submit = new Subject<void>();
  protected form: FormGroup;

  protected amountKeys: string[] = ['1'];

  constructor(
    private readonly formBuilder: FormBuilder,
    @Inject(CURRENT_CONFIGURATION) config: ReceiptsDocumentConfiguration
  ) {
    super();

    if (!config.amounts) {
      throw new Error('Config amounts may not be undefined.');
    }

    const amountGroups: { [key: string]: unknown } = {};
    this.amountKeys = Object.keys(config.amounts);
    for (const key of this.amountKeys) {
      amountGroups[key] = formBuilder.group({
        amount: [config.amounts[key].amount, [Validators.required, Validators.min(0.01)]]
      });
    }

    this.form = formBuilder.group({
      amounts: formBuilder.group(amountGroups),
      currency: [config.currency, [Validators.required, Validators.maxLength(10)]],
      headerInfo: [config.headerInfo, Validators.maxLength(100)],
      signatureLocation: [config.signatureLocation, Validators.maxLength(100)],
      signatureRecipient: [config.signatureRecipient, Validators.maxLength(100)],
      showSponsorLogo: [config.showSponsorLogo],
      combineSimilarTeams: [config.combineSimilarTeams]
    });
  }

  protected get amounts(): FormGroup {
    return this.form.get('amounts')! as FormGroup;
  }

  protected get baseAmount(): AbstractControl {
    return this.amounts.get('1')!;
  }

  protected get currency(): AbstractControl {
    return this.form.get('currency')!;
  }

  protected get headerInfo(): AbstractControl {
    return this.form.get('headerInfo')!;
  }

  protected get signatureLocation(): AbstractControl {
    return this.form.get('signatureLocation')!;
  }

  protected get signatureRecipient(): AbstractControl {
    return this.form.get('signatureRecipient')!;
  }

  protected get combineSimilarTeams(): AbstractControl {
    return this.form.get('combineSimilarTeams')!;
  }

  protected get canAddReducedAmountEntry(): boolean {
    return this.amountKeys.length < DocumentConfigReceiptsComponent.maxNumberOfAmountEntries;
  }

  public getConfig(): ReceiptsDocumentConfiguration {
    return this.form.value as ReceiptsDocumentConfiguration;
  }

  public isValid(): boolean {
    return this.form.valid;
  }

  public ngOnDestroy(): void {
    this.submit.complete();
  }

  protected amount(key: string): AbstractControl {
    return this.amounts.get(key)!;
  }

  protected addReducedAmountEntry(): void {
    if (!this.canAddReducedAmountEntry) {
      return;
    }

    // Entries need not be consecutive, this should probably be possible through the UI at some point.
    // For now, we simply find the lowest number that is not currently a key in the map.
    const existingEntries = this.amountKeys.map((x) => +x);
    let newEntry = 2;
    while (existingEntries.some((x) => x === newEntry)) {
      newEntry++;
    }

    this.amounts.addControl(
      `${newEntry}`,
      this.formBuilder.group({
        amount: [10, [Validators.required, Validators.min(0.01)]]
      })
    );

    this.amountKeys.push(`${newEntry}`);
    this.amountKeys.sort((a, b) => +a - +b);
  }

  protected removeReducedAmountEntry(amountKey: string): void {
    if (amountKey === '1') {
      return;
    }

    this.amounts.removeControl(amountKey);
    this.amountKeys = this.amountKeys.filter((x) => x !== amountKey);
  }
}
