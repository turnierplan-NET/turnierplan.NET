import { Component, Input } from '@angular/core';
import { LabelDto } from '../../../api/models/label-dto';

@Component({
  selector: 'tp-label',
  imports: [],
  templateUrl: './label.component.html'
})
export class LabelComponent {
  protected _label!: LabelDto;
  protected _borderColor: string = '000';
  protected _textColor: string = '000';

  @Input()
  public set label(value: LabelDto) {
    this._label = value;
    this.processColor();
  }

  private processColor(): void {
    const rgb = [
      parseInt(this._label.colorCode.substring(0, 2), 16),
      parseInt(this._label.colorCode.substring(2, 4), 16),
      parseInt(this._label.colorCode.substring(4, 6), 16)
    ];

    const toHex = (v: number): string => Math.floor(v).toString(16).padStart(2, '0');

    this._borderColor = rgb.map((v) => toHex((v * 4) / 5)).join('');
    this._textColor = (rgb[0] + rgb[1] + rgb[2]) / 3 > 170 ? '000' : 'fff';
  }
}
