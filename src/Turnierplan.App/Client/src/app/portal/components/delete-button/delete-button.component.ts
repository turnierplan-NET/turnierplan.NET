import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { NgClass } from '@angular/common';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'tp-delete-button',
    templateUrl: './delete-button.component.html',
    imports: [NgbPopover, NgClass, TranslateDirective, TranslatePipe]
})
export class DeleteButtonComponent {
  @Input()
  public reducedFootprint: boolean = false;

  @Input()
  public showLabel: boolean = false;

  @Input()
  public disabled: boolean = false;

  @Output()
  public confirmed = new EventEmitter<void>();
}
