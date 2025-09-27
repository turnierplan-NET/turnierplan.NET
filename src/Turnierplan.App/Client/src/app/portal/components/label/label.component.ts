import { Component, Input } from '@angular/core';
import { LabelDto } from '../../../api/models/label-dto';

@Component({
  selector: 'tp-label',
  imports: [],
  templateUrl: './label.component.html'
})
export class LabelComponent {
  @Input()
  public label!: LabelDto;

  protected getBorderColor(color: string): string {
    const rgb = this.toRgb(color);
    return rgb.map((v) => this.toHex((v * 4) / 5)).join('');
  }

  protected getTextColor(color: string): string {
    const rgb = this.toRgb(color);
    return (rgb[0] + rgb[1] + rgb[2]) / 3 > 170 ? '000' : 'fff';
  }

  private toRgb(color: string): [number, number, number] {
    return [
      Number.parseInt(color.substring(0, 2), 16),
      Number.parseInt(color.substring(2, 4), 16),
      Number.parseInt(color.substring(4, 6), 16)
    ];
  }

  private toHex(value: number): string {
    return Math.floor(value).toString(16).padStart(2, '0');
  }
}
