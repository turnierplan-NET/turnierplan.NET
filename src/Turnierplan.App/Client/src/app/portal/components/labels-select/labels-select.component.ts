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
  protected selectedLabelIds = new Set<number>();

  constructor(protected readonly modal: NgbActiveModal) {}

  public init(availableLabels: LabelDto[], selectedLabelIds: number[]): void {
    if (this.isInitialized) {
      return;
    }

    this.availableLabels = availableLabels;
    this.selectedLabelIds = new Set<number>(selectedLabelIds);

    this.isInitialized = true;
  }

  protected setLabelSelected(id: number, selected: boolean): void {
    if (selected) {
      this.selectedLabelIds.add(id);
    } else {
      this.selectedLabelIds.delete(id);
    }
  }

  protected save(): void {
    this.modal.close([...this.selectedLabelIds]);
  }
}
