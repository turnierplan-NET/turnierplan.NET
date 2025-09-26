import { Component } from '@angular/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';
import { LabelDto } from '../../../api/models/label-dto';
import { FormsModule } from '@angular/forms';
import { LabelComponent } from '../label/label.component';

@Component({
  imports: [ActionButtonComponent, TranslateDirective, FormsModule, LabelComponent],
  templateUrl: './labels-select.component.html'
})
export class LabelsSelectComponent {
  protected isInitialized = false;
  protected availableLabels: LabelDto[] = [];
  protected labelsSelected: { [key: string]: boolean } = {};

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(availableLabels: LabelDto[], selectedLabelIds: number[]): void {
    if (this.isInitialized) {
      return;
    }

    this.availableLabels = availableLabels;
    this.labelsSelected = {};

    for (const label of this.availableLabels) {
      this.labelsSelected[label.id] = selectedLabelIds.some((x) => x === label.id);
    }

    this.isInitialized = true;
  }

  protected save(): void {
    this.modal.close(this.availableLabels.map((label) => label.id).filter((id) => this.labelsSelected[id]));
  }
}
