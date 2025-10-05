import { Component, Input } from '@angular/core';
import { LabelComponent } from '../label/label.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { ApplicationChangeLogType } from '../../../api/models/application-change-log-type';
import { ApplicationChangeLogProperty } from '../../../api/models/application-change-log-property';
import { ApplicationChangeLogDto } from '../../../api/models/application-change-log-dto';
import { LabelDto } from '../../../api/models/label-dto';
import { TranslateDirective } from '@ngx-translate/core';

@Component({
  selector: 'tp-application-change-log-entry',
  imports: [LabelComponent, TranslateDatePipe, TranslateDirective],
  templateUrl: './application-change-log-entry.component.html',
  styleUrl: './application-change-log-entry.component.scss'
})
export class ApplicationChangeLogEntryComponent {
  @Input()
  public index!: number;

  @Input()
  public set entry(value: ApplicationChangeLogDto) {
    this.changeLogType = value.type;
    this.changeLogTimestamp = value.timestamp;

    const propertyValue = (type: ApplicationChangeLogProperty): string => {
      return value.properties.find((x) => x.type === type)?.value ?? '';
    };

    switch (value.type) {
      case ApplicationChangeLogType.NotesChanged:
      case ApplicationChangeLogType.CommentChanged:
      case ApplicationChangeLogType.ContactChanged:
      case ApplicationChangeLogType.ContactEmailChanged:
      case ApplicationChangeLogType.ContactTelephoneChanged:
      case ApplicationChangeLogType.TeamRenamed: {
        this.changeLogIcon = 'bi-pencil-square';
        this.previousValue = propertyValue(ApplicationChangeLogProperty.PreviousValue);
        this.newValue = propertyValue(ApplicationChangeLogProperty.NewValue);
        break;
      }
      case ApplicationChangeLogType.TeamAdded: {
        this.changeLogIcon = 'bi-plus-square';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        break;
      }
      case ApplicationChangeLogType.TeamRemoved: {
        this.changeLogIcon = 'bi-dash-square';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        break;
      }
      case ApplicationChangeLogType.LabelAdded:
      case ApplicationChangeLogType.LabelRemoved: {
        this.changeLogIcon = 'bi-tags';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        this.mockLabel = {
          id: 0,
          name: propertyValue(ApplicationChangeLogProperty.LabelName),
          colorCode: propertyValue(ApplicationChangeLogProperty.LabelColorCode),
          description: ''
        };
        break;
      }
    }
  }

  protected readonly ApplicationChangeLogType = ApplicationChangeLogType;

  protected changeLogType?: ApplicationChangeLogType;
  protected changeLogTimestamp: string = '';
  protected changeLogIcon: string = '';

  protected previousValue?: string;
  protected newValue?: string;
  protected teamName?: string;
  protected mockLabel?: LabelDto;
}
