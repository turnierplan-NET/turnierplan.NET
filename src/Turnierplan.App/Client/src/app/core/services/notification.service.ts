import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  constructor(
    private readonly toastr: ToastrService,
    private readonly translator: TranslateService
  ) {}

  public showNotification(
    type: 'success' | 'warning' | 'error' | 'info',
    title: string,
    message: string,
    timeout: number = 5000,
    titleParams: { [key: string]: string } = {},
    messageParams: { [key: string]: string } = {}
  ): void {
    this.toastr[type](
      (this.translator.instant(message, messageParams) ?? message) as string,
      (this.translator.instant(title, titleParams) ?? title) as string,
      {
        timeOut: timeout,
        closeButton: true
      }
    );
  }
}
