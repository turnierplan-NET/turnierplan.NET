import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TranslateDirective } from '@ngx-translate/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateResourceGroupEndpointRequest } from '../../../api/models/create-resource-group-endpoint-request';

@Component({
  selector: 'tp-new-resource-group-dialog',
  imports: [TranslateDirective, ActionButtonComponent, FormsModule, ReactiveFormsModule],
  changeDetection: ChangeDetectionStrategy.Eager,
  templateUrl: './new-resource-group-dialog.component.html'
})
export class NewResourceGroupDialogComponent {
  protected readonly form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
    description: new FormControl('', { nonNullable: true }),
    start: new FormControl<string>('', { nonNullable: true }),
    end: new FormControl<string>('', { nonNullable: true })
  });

  constructor(protected readonly modal: NgbActiveModal) {}

  protected submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();
    const description = value.description.trim();

    const request: CreateResourceGroupEndpointRequest = {
      name: value.name.trim(),
      description: description.length === 0 ? undefined : description,
      start: undefined, // TODO
      end: undefined // TODO
    };

    this.modal.close(request);
  }
}
