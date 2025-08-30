import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'tp-illustration',
  templateUrl: './illustration.component.html',
  styleUrls: ['./illustration.component.scss'],
  imports: [NgClass, TranslatePipe]
})
export class IllustrationComponent {
  @Input()
  public name!: string;

  @Input()
  public small: boolean = false;
}
