import { Component, EventEmitter, inject, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { ColorThemeService } from '../../../core/services/color-theme.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'tp-action-button',
  templateUrl: './action-button.component.html',
  imports: [NgClass, TranslateDirective, TranslatePipe]
})
export class ActionButtonComponent implements OnInit, OnDestroy {
  protected processedType: string = '';

  @Input()
  public icon?: string;

  @Input()
  public type = 'outline-primary';

  @Input()
  public title = '';

  @Input()
  public titleParams: { [key: string]: unknown } = {};

  @Input()
  public mode: 'IconLeftAndText' | 'IconRightAndText' | 'IconOnly' = 'IconLeftAndText';

  @Input()
  public disabled = false;

  @Output()
  public buttonClick = new EventEmitter<void>();

  private readonly colorThemeService = inject(ColorThemeService);
  private readonly destroyed$ = new Subject<void>();

  public ngOnInit(): void {
    if (this.type === 'outline-dark' || this.type === 'dark' || this.type === 'outline-light' || this.type === 'light') {
      // When dark mode is enabled, certain button styles don't have sufficient contrast or are "too bright".
      // These four button styles are converted to the outline / non-outline secondary version.
      this.colorThemeService.isDarkMode$.pipe(takeUntil(this.destroyed$)).subscribe({
        next: (isDarkMode) => {
          this.processedType = isDarkMode ? this.type.replace(/dark|light/, 'secondary') : this.type;
        }
      });
    } else {
      this.processedType = this.type;
    }
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
