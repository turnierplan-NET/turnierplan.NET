import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject } from 'rxjs';

import { NullableOfPublicId, VenueDto, VenuesService } from '../../../api';

@Component({
  templateUrl: './venue-select.component.html'
})
export class VenueSelectComponent {
  public selected$ = new Subject<{ id: string; name: string } | undefined>();

  protected isLoading = true;
  protected isSaving = false;
  protected initialVenueId: string = '';
  protected currentVenueId: string = '';
  protected venues: VenueDto[] = [];

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly venueService: VenuesService
  ) {}

  public initialize(organizationId: string, currentVenueId?: NullableOfPublicId): void {
    this.venueService.getVenues({ organizationId: organizationId }).subscribe({
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
