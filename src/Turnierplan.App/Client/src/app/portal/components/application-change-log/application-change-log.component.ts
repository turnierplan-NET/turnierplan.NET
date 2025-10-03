import { Component } from '@angular/core';
import { ApplicationDto } from '../../../api/models/application-dto';

@Component({
  selector: 'tp-application-change-log',
  imports: [],
  templateUrl: './application-change-log.component.html',
  styleUrl: './application-change-log.component.scss'
})
export class ApplicationChangeLogComponent {
  public init(application: ApplicationDto): void {}
}
