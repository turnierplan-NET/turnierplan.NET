import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'tp-root',
  templateUrl: './app.component.html',
  imports: [RouterOutlet]
})
export class AppComponent {
  private translateService = inject(TranslateService);

  constructor() {
    this.translateService.setDefaultLang('de');
    this.translateService.use('de');
  }
}
