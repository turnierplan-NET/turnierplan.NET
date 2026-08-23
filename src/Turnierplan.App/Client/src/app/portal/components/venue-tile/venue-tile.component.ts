import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

import { TranslateDirective } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { VenueDto } from '../../../api/models/venue-dto';

@Component({
  selector: 'tp-venue-tile',
  templateUrl: './venue-tile.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [TranslateDirective, RouterLink]
})
export class VenueTileComponent {
  @Input()
  public venue?: VenueDto;
}
