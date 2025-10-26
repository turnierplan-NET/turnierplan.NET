import { AfterViewInit, Component, ElementRef, EventEmitter, inject, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AlertComponent } from '../alert/alert.component';
import { TranslateDirective } from '@ngx-translate/core';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { ColorThemeService } from '../../../core/services/color-theme.service';

@Component({
  selector: 'tp-unsaved-changes-alert',
  templateUrl: './unsaved-changes-alert.component.html',
  imports: [AlertComponent, TranslateDirective, SmallSpinnerComponent, ActionButtonComponent]
})
export class UnsavedChangesAlertComponent implements OnInit, AfterViewInit, OnDestroy {
  @Input()
  public inProgress: boolean = false;

  @Output()
  public save = new EventEmitter<void>();

  @ViewChild('wrapperDiv')
  protected wrapperDiv!: ElementRef<HTMLDivElement>;

  private readonly colorThemeService = inject(ColorThemeService);
  private readonly destroyed$ = new Subject<void>();
  private readonly eventListener = () => this.update();

  private currentTranslateY = 0;
  private shadowColor = '#fff';

  public ngOnInit(): void {
    window.addEventListener('scroll', this.eventListener);
  }

  public ngOnDestroy(): void {
    window.removeEventListener('scroll', this.eventListener);

    this.destroyed$.next();
    this.destroyed$.complete();
  }

  public ngAfterViewInit(): void {
    this.colorThemeService.isDarkMode$.pipe(takeUntil(this.destroyed$)).subscribe((isDarkMode) => {
      this.shadowColor = isDarkMode ? '#212529' : '#fff';
      this.update();
    });
  }

  protected update(): void {
    const y = this.wrapperDiv.nativeElement.getBoundingClientRect().top - this.currentTranslateY;
    let translateY = 0;

    if (y < 70) {
      translateY = 70 - y;
    }

    this.wrapperDiv.nativeElement.style.transform = `translateY(${translateY}px)`;

    let shadow = Math.floor(Math.min(translateY / 3, 15));
    this.wrapperDiv.nativeElement.style.boxShadow = `0 0 ${shadow}px ${shadow}px ${this.shadowColor}`;

    // z-index must be < 1000, otherwise we would end up above the modal backdrop
    this.wrapperDiv.nativeElement.style.zIndex = translateY === 0 ? '0' : '800';

    let margin = Math.floor(Math.min(translateY / 4, 10));
    this.wrapperDiv.nativeElement.style.marginLeft = `${margin}px`;
    this.wrapperDiv.nativeElement.style.marginRight = `${margin}px`;

    this.currentTranslateY = translateY;
  }
}
