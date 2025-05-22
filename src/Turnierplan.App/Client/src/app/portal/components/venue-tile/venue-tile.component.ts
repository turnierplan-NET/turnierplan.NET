import { Component, Input } from '@angular/core';

import { VenueDto } from '../../../api';

@Component({
  selector: 'tp-venue-tile',
  templateUrl: './venue-tile.component.html'
})
export class VenueTileComponent {
  @Input()
  public venue?: VenueDto;
}
