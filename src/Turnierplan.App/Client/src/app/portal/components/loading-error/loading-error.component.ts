import { Component, Input } from '@angular/core';
import { ErrorPageComponent } from '../error-page/error-page.component';
import { TranslateDirective } from '@ngx-translate/core';

@Component({
  templateUrl: './loading-error.component.html',
  styleUrls: ['./loading-error.component.scss'],
  imports: [ErrorPageComponent, TranslateDirective]
})
export class LoadingErrorComponent {
  @Input()
  public error: unknown;

  protected get errorDescription(): string {
    return JSON.stringify(this.error, null, 2);
  }

  protected get statusCode(): number | undefined {
    return this.error === undefined ? undefined : (this.error as { status?: number }).status;
  }
}
