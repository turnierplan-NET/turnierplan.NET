import { Component, EventEmitter, Input, Output } from '@angular/core';

export type MultiSelectFilterOption = {
  value: unknown;
  label: string;
};

@Component({
  standalone: false,
  selector: 'tp-multi-select-filter',
  templateUrl: './multi-select-filter.component.html'
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
    return this._selected.some((x) => x === value);
  }

  protected setValueSelected(value: unknown, isSelected: boolean): void {
    let updated = false;

    if (isSelected) {
      if (!this._selected.some((x) => x === value)) {
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
