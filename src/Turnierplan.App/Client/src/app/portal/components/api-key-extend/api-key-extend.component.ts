import { Component, OnDestroy } from '@angular/core';
import { ApiKeyDto } from '../../../api/models/api-key-dto';
import { Observable, Subject } from 'rxjs';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgbActiveOffcanvas } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';
import { LoadingIndicatorComponent } from '../loading-indicator/loading-indicator.component';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { setApiKeyExpiryDate } from '../../../api/fn/api-keys/set-api-key-expiry-date';
import { AlertComponent } from '../alert/alert.component';

@Component({
  imports: [
    ActionButtonComponent,
    TranslateDirective,
    LoadingIndicatorComponent,
    FormsModule,
    ReactiveFormsModule,
    TranslateDatePipe,
    AlertComponent
  ],
  templateUrl: './api-key-extend.component.html'
})
export class ApiKeyExtendComponent implements OnDestroy {
  protected isSubmitting = false;
  protected validityOptions: number[] = [];
  protected validityControl = new FormControl<number>(30, { nonNullable: true });
  protected _apiKey?: ApiKeyDto;
  protected newValidUntil: Date = new Date();

  private readonly errorSubject$ = new Subject<unknown>();
  private readonly now = Date.now();

  constructor(
    protected readonly offcanvas: NgbActiveOffcanvas,
    private readonly turnierplanApi: TurnierplanApi
  ) {
    this.calculateNewValidUntil();
    this.validityControl.valueChanges.pipe(takeUntilDestroyed()).subscribe({
      next: () => this.calculateNewValidUntil()
    });
  }

  public set apiKey(value: ApiKeyDto) {
    this._apiKey = value;

    // Add 1 hour worth of clock skew
    const apiKeyExpireTime = new Date(value.expiryDate).getTime() + 60 * 60 * 1000;

    this.validityOptions = [1, 7, 30, 90, 180, 365].filter((x) => {
      const validUntil = new Date(this.now + x * 86400000);
      return validUntil.getTime() > apiKeyExpireTime;
    });
    this.validityControl.setValue(this.validityOptions.length === 0 ? 0 : this.validityOptions[0]);
  }

  public get error$(): Observable<unknown> {
    return this.errorSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.errorSubject$.complete();
  }

  protected confirmClicked(): void {
    if (this._apiKey) {
      this.isSubmitting = true;
      this.turnierplanApi.invoke(setApiKeyExpiryDate, { id: this._apiKey.id, body: { validity: this.validityControl.value } }).subscribe({
        next: () => this.offcanvas.close(),
        error: (error) => this.errorSubject$.next(error)
      });
    }
  }

  private calculateNewValidUntil(): void {
    this.newValidUntil = new Date(this.now + this.validityControl.value * 86400000);
  }
}
