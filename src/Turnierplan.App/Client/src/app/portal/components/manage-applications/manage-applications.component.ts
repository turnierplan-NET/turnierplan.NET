import { Component, Input } from '@angular/core';
import { ApplicationsFilter } from '../../models/applications-filter';
import { ReplaySubject } from 'rxjs';

@Component({
  standalone: false,
  selector: 'tp-manage-applications',
  templateUrl: './manage-applications.component.html'
})
export class ManageApplicationsComponent {
  // TODO private?
  protected readonly filter$ = new ReplaySubject<ApplicationsFilter>();

  @Input()
  public set filter(value: ApplicationsFilter) {
    this.filter$.next(value);
  }
}
