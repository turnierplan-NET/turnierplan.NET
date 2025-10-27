import { Component, inject } from '@angular/core';
import { NotificationService } from '../../services/notification.service';
import { AsyncPipe } from '@angular/common';
import { NgbToast } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'tp-notifications',
  imports: [AsyncPipe, NgbToast],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss'
})
export class NotificationsComponent {
  protected readonly notificationService = inject(NotificationService);
}
