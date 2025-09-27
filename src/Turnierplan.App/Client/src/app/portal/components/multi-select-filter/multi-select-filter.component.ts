import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbDropdown, NgbDropdownToggle, NgbDropdownMenu, NgbDropdownItem } from '@ng-bootstrap/ng-bootstrap';
import { NgClass } from '@angular/common';
import { TranslateDirective } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';

export type MultiSelectFilterOption = {
  value: unknown;
  label: string;
};

@Component({
  selector: 'tp-multi-select-filter',
  templateUrl: './multi-select-filter.component.html',
  imports: [NgbDropdown, NgbDropdownToggle, NgClass, TranslateDirective, NgbDropdownMenu, FormsModule, NgbDropdownItem]
})
export class MultiSelectFilterComponent {
  private _selected: unknown[] = [];

  @Input()
  public title!: string;

  @Input()
  public idPrefix!: string;

  @Input()
  public options: MultiSelectFilterOption[] = [];

  @Input()
  public specialOption?: MultiSelectFilterOption;

  @Input()
  public set selected(value: unknown[]) {
    this._selected = value.filter((value) => value === this.specialOption?.value || this.options.some((option) => option.value === value));

    if (this._selected.length !== value.length) {
      this.selectedChange.emit(this._selected);
    }
  }

  @Output()
  public selectedChange = new EventEmitter<unknown[]>();

  protected get totalCount(): number {
    return this.options.length + (this.specialOption ? 1 : 0);
  }

  protected get allSelected(): boolean {
    return this.selectedCount === 0 || this.selectedCount == this.totalCount;
  }

  protected get selectedCount(): number {
    return this._selected.length;
  }

  protected isValueSelected(value: unknown): boolean {
    return this._selected.includes(value);
  }

  protected setValueSelected(value: unknown, isSelected: boolean): void {
    let updated = false;

    if (isSelected) {
      if (!this._selected.includes(value)) {
        this._selected.push(value);
        updated = true;
      }
    } else {
      const index = this._selected.indexOf(value);
      if (index !== -1) {
        this._selected.splice(index, 1);
        updated = true;
      }
    }

    if (updated) {
      this.selectedChange.emit(this._selected);
    }
  }

  protected unsetAllValues(): void {
    if (this._selected.length === 0) {
      return;
    }

    this._selected = [];
    this.selectedChange.emit(this._selected);
  }
}
