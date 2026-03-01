import { Component, inject, Input } from '@angular/core';
import { EChartsCoreOption } from 'echarts/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { ColorThemeService } from '../../../core/services/color-theme.service';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'tp-chart-wrapper',
  imports: [NgxEchartsDirective, AsyncPipe],
  templateUrl: './chart-wrapper.component.html'
})
export class ChartWrapperComponent {
  @Input({ required: true })
  public chartOptions!: EChartsCoreOption;

  protected readonly colorThemeService = inject(ColorThemeService);

  protected style: { [key: string]: string } = {};

  private _width?: number;
  private _height?: number;

  @Input()
  public set width(value: number) {
    this._width = value;
    this.buildStyle();
  }

  @Input()
  public set height(value: number) {
    this._height = value;
    this.buildStyle();
  }

  private buildStyle(): void {
    this.style = {};

    if (this._width !== undefined) {
      this.style['width'] = `${this._width}px`;
    }

    if (this._height !== undefined) {
      this.style['height'] = `${this._height}px`;
    }
  }
}
