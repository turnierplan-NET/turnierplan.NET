import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
    templateUrl: './text-list-dialog.component.html',
    imports: [TranslateDirective, FormsModule, ReactiveFormsModule, NgClass, ActionButtonComponent]
})
export class TextListDialogComponent {
  protected isInitialized = false;
  protected translationKey: string = '';
  protected itemRegex?: RegExp;
  protected initialValue: string[] = [];
  protected formArray!: FormArray;

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly formBuilder: FormBuilder
  ) {}

  public init(translationKey: string, itemRegex: RegExp | undefined, initialValue: string[]): void {
    if (this.isInitialized) {
      return;
    }

    this.translationKey = translationKey;
    this.itemRegex = itemRegex;
    this.initialValue = [...initialValue];

    this.formArray = this.formBuilder.array(this.initialValue.map((x) => this.createControl(x)));

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
    return this.formBuilder.control(
      value,
      this.itemRegex ? Validators.compose([Validators.required, Validators.pattern(this.itemRegex)]) : Validators.required
    );
  }

  protected addEmptyControl(): void {
    this.formArray.push(this.createControl(''));
  }

  protected removeControlAtIndex(index: number): void {
    this.formArray.removeAt(index);
  }
}
