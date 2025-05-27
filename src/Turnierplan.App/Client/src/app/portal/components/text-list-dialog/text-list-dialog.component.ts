import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  standalone: false,
  templateUrl: './text-list-dialog.component.html'
})
export class TextListDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected maxItemCount: number = 0;
  protected maxItemLength: number = 0;
  protected initialValue: string[] = [];

  protected formArray!: FormArray;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly formBuilder: FormBuilder
  ) {}

  public init(translationKey: string, maxItemCount: number, maxItemLength: number, initialValue: string[]): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.maxItemCount = maxItemCount;
    this.maxItemLength = maxItemLength;
    this.initialValue = [...initialValue];

    this.formArray = this.formBuilder.array(
      this.initialValue.map((x) => this.createControl(x)),
      Validators.maxLength(maxItemCount)
    );

    this.isInitialized = true;
  }

  protected save(): void {
    this.formArray.updateValueAndValidity();
    this.formArray.markAllAsTouched();

    if (!this.formArray.valid) {
      return;
    }

    const sanitized = (this.formArray.value as string[]).map((x) => x.trim());
    const sanitizedInitial = this.initialValue.map((x) => x.trim());

    if (sanitized.length !== sanitizedInitial.length || sanitized.some((x, i) => x !== sanitizedInitial[i])) {
      this.modal.close(sanitized);
    } else {
      this.modal.dismiss();
    }
  }

  protected getControls(): FormControl[] {
    return this.formArray.controls as FormControl[];
  }

  protected createControl(value: string): FormControl {
    const validators = [Validators.required, Validators.maxLength(this.maxItemLength)];

    return this.formBuilder.control(value, Validators.compose(validators));
  }

  protected addEmptyControl(): void {
    if (this.formArray.length >= this.maxItemCount) {
      return;
    }

    this.formArray.push(this.createControl(''));
  }

  protected removeControlAtIndex(index: number): void {
    this.formArray.removeAt(index);
  }
}
