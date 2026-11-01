import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ColorThemeService } from './core/services/color-theme.service';
import { NotificationsComponent } from './core/components/notifications/notifications.component';
import { AuthenticationService } from './core/services/authentication.service';

@Component({
  selector: 'tp-root',
  templateUrl: './app.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterOutlet, NotificationsComponent]
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);

  constructor() {
    inject(ColorThemeService).initialize();
    inject(AuthenticationService).initialize();

    this.translateService.setFallbackLang('de');
    this.translateService.use('de');
  }
}
