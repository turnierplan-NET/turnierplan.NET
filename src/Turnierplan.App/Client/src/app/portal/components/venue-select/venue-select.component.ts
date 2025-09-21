import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

import { TranslateDirective } from '@ngx-translate/core';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { FormsModule } from '@angular/forms';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { NgClass } from '@angular/common';
import { VenueDto } from '../../../api/models/venue-dto';
import { NullableOfPublicId } from '../../../api/models/nullable-of-public-id';
import { getVenues } from '../../../api/fn/venues/get-venues';
import { TurnierplanApi } from '../../../api/turnierplan-api';

@Component({
  templateUrl: './venue-select.component.html',
  imports: [TranslateDirective, SmallSpinnerComponent, FormsModule, ActionButtonComponent, NgClass]
})
export class VenueSelectComponent {
  public selected$ = new Subject<{ id: string; name: string } | undefined>();

  protected isLoading = true;
  protected isSaving = false;
  protected initialVenueId: string = '';
  protected currentVenueId: string = '';
  protected venues: VenueDto[] = [];

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    protected readonly modal: NgbActiveModal
  ) {}

  public initialize(organizationId: string, currentVenueId?: NullableOfPublicId): void {
    this.turnierplanApi.invoke(getVenues, { organizationId: organizationId }).subscribe({
      next: (venues) => {
        this.venues = venues;
        this.initialVenueId = currentVenueId ?? '';
        this.currentVenueId = currentVenueId ?? '';
        this.isLoading = false;
      },
      error: (error) => {
        this.selected$.error(error);
      }
    });
  }

  protected saveVenue(): void {
    if (this.isLoading || this.isSaving) {
      return;
    }

    if (this.initialVenueId === this.currentVenueId) {
      this.modal.dismiss();
      return;
    }

    this.isSaving = true;

    if (this.currentVenueId === '') {
      this.selected$.next(undefined);
    } else {
      this.selected$.next({
        id: this.currentVenueId,
        name: this.venues.find((x) => x.id === this.currentVenueId)?.name ?? ''
      });
    }

    this.selected$.complete();
  }
}
