import { Component, Input } from '@angular/core';

@Component({
  templateUrl: './loading-error.component.html',
  styleUrls: ['./loading-error.component.scss']
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
