import { Component, Input } from '@angular/core';

import { environment } from '../../../../environments/environment';
import { NgTemplateOutlet, NgClass } from '@angular/common';
import { TranslateDirective } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'tp-footer',
    templateUrl: './footer.component.html',
    imports: [NgTemplateOutlet, TranslateDirective, RouterLink, NgClass]
})
export class FooterComponent {
  @Input()
  public contentOnly = false;

  @Input()
  public footerStyle: string = '';

  protected get version(): string {
    return environment.version;
  }

  protected get isDevelopment(): boolean {
    return !environment.production;
  }
}
