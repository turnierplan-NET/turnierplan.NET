import { Component, Input } from '@angular/core';

@Component({
  selector: 'tp-illustration',
  templateUrl: './illustration.component.html',
  styleUrls: ['./illustration.component.scss']
})
export class IllustrationComponent {
  @Input()
  public name!: string;

  @Input()
  public small: boolean = false;
}
