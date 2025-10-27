import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BehaviorSubject, Observable } from 'rxjs';

export type NotificationType = 'success' | 'warning' | 'danger' | 'info';

export interface Notification {
  id: number;
  type: NotificationType;
  title: string;
  body: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly notificationsSubject$ = new BehaviorSubject<Notification[]>([]);
  private readonly translateService = inject(TranslateService);

  private nextNotificationId = 1;

  public get notifications$(): Observable<Notification[]> {
    return this.notificationsSubject$.asObservable();
  }

  public showNotification(
    type: NotificationType,
    title: string,
    body: string,
    titleParams: { [key: string]: string } = {},
    bodyParams: { [key: string]: string } = {}
  ): void {
    const notification: Notification = {
      id: this.nextNotificationId++,
      type: type,
      title: (this.translateService.instant(title, titleParams) ?? title) as string,
      body: (this.translateService.instant(body, bodyParams) ?? body) as string
    };

    this.notificationsSubject$.next([...this.notificationsSubject$.value, notification]);
  }

  public removeNotification(id: number): void {
    this.notificationsSubject$.next(this.notificationsSubject$.value.filter((notification) => notification.id !== id));
  }
}
