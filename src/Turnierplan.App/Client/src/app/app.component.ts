import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ColorThemeService } from './core/services/color-theme.service';
import { NotificationsComponent } from './core/components/notifications/notifications.component';

@Component({
  selector: 'tp-root',
  templateUrl: './app.component.html',
  imports: [RouterOutlet, NotificationsComponent]
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);

  constructor() {
    inject(ColorThemeService).initialize();

    this.translateService.setFallbackLang('de');
    this.translateService.use('de');
  }
}
