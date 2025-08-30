import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { AlertComponent } from '../alert/alert.component';
import { TranslateDirective } from '@ngx-translate/core';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { ActionButtonComponent } from '../action-button/action-button.component';

@Component({
  selector: 'tp-unsaved-changes-alert',
  templateUrl: './unsaved-changes-alert.component.html',
  imports: [AlertComponent, TranslateDirective, SmallSpinnerComponent, ActionButtonComponent]
})
export class UnsavedChangesAlertComponent implements OnInit, OnDestroy {
  @Input()
  public inProgress: boolean = false;

  @Output()
  public save = new EventEmitter<void>();

  @ViewChild('wrapperDiv')
  protected wrapperDiv!: ElementRef<HTMLDivElement>;

  private readonly eventListener = () => this.update();
  private currentTranslateY = 0;

  public ngOnInit(): void {
    window.addEventListener('scroll', this.eventListener);
    setTimeout(() => this.update(), 0);
  }

  public ngOnDestroy(): void {
    window.removeEventListener('scroll', this.eventListener);
  }

  protected update(): void {
    const y = this.wrapperDiv.nativeElement.getBoundingClientRect().top - this.currentTranslateY;
    let translateY = 0;

    if (y < 70) {
      translateY = 70 - y;
    }

    this.wrapperDiv.nativeElement.style.transform = `translateY(${translateY}px)`;

    let shadow = Math.floor(Math.min(translateY / 3, 15));
    this.wrapperDiv.nativeElement.style.boxShadow = `0 0 ${shadow}px 5px #fff`;

    let margin = Math.floor(Math.min(translateY / 4, 10));
    this.wrapperDiv.nativeElement.style.marginLeft = `${margin}px`;
    this.wrapperDiv.nativeElement.style.marginRight = `${margin}px`;

    this.currentTranslateY = translateY;
  }
}
