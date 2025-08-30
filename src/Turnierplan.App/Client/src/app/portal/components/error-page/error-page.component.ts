import { Component, Input } from '@angular/core';
import { IllustrationComponent } from '../illustration/illustration.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'tp-error-page',
    templateUrl: './error-page.component.html',
    styleUrls: ['./error-page.component.scss'],
    imports: [IllustrationComponent, ActionButtonComponent, RouterLink]
})
export class ErrorPageComponent {
  @Input()
  public illustrationName?: string;

  @Input()
  public actionButton?: { label: string; icon: string; routerLink: string };
}
