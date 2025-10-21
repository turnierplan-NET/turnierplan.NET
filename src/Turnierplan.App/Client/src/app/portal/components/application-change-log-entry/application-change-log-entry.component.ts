import { Component, Input } from '@angular/core';
import { LabelComponent } from '../label/label.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { ApplicationChangeLogType } from '../../../api/models/application-change-log-type';
import { ApplicationChangeLogProperty } from '../../../api/models/application-change-log-property';
import { ApplicationChangeLogDto } from '../../../api/models/application-change-log-dto';
import { LabelDto } from '../../../api/models/label-dto';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'tp-application-change-log-entry',
  imports: [LabelComponent, TranslateDatePipe, TranslateDirective, NgbTooltip, TranslatePipe],
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
        this.changeLogIcon = 'bi-plus-circle';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        this.tournamentClassName = propertyValue(ApplicationChangeLogProperty.TournamentClassName);
        break;
      }
      case ApplicationChangeLogType.TeamRemoved: {
        this.changeLogIcon = 'bi-trash';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        this.tournamentClassName = propertyValue(ApplicationChangeLogProperty.TournamentClassName);
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
      case ApplicationChangeLogType.TeamLinkCreated: {
        this.changeLogIcon = 'bi-link-45deg';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        this.tournamentName = propertyValue(ApplicationChangeLogProperty.TournamentName);
        break;
      }
      case ApplicationChangeLogType.TeamLinkDestroyed: {
        this.changeLogIcon = 'bi-lightning-charge';
        this.teamName = propertyValue(ApplicationChangeLogProperty.TeamName);
        this.tournamentName = propertyValue(ApplicationChangeLogProperty.TournamentName);
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
  protected tournamentName?: string;
  protected tournamentClassName?: string;
}
