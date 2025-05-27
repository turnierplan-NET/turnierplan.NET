import { Component, Input } from '@angular/core';

import { environment } from '../../../../environments/environment';

@Component({
  standalone: false,
  selector: 'tp-footer',
  templateUrl: './footer.component.html'
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
