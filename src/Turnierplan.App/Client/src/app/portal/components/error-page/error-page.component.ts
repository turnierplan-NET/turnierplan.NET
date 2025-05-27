import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'tp-error-page',
  templateUrl: './error-page.component.html',
  styleUrls: ['./error-page.component.scss']
})
export class ErrorPageComponent {
  @Input()
  public illustrationName?: string;

  @Input()
  public actionButton?: { label: string; icon: string; routerLink: string };
}
