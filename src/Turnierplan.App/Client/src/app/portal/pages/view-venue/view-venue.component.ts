import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { of, Subject, switchMap, takeUntil } from 'rxjs';

import { VenueDto } from '../../../api';
import { DiscardChangesDetector } from '../../../core/guards/discard-changes.guard';
import { NotificationService } from '../../../core/services/notification.service';
import { PageFrameNavigationTab, PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TextInputDialogComponent } from '../../components/text-input-dialog/text-input-dialog.component';
import { TextListDialogComponent } from '../../components/text-list-dialog/text-list-dialog.component';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { Actions } from '../../../generated/actions';
import { UnsavedChangesAlertComponent } from '../../components/unsaved-changes-alert/unsaved-changes-alert.component';
import { AlertComponent } from '../../components/alert/alert.component';
import { TranslateDirective } from '@ngx-translate/core';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { RenameButtonComponent } from '../../components/rename-button/rename-button.component';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { TooltipIconComponent } from '../../components/tooltip-icon/tooltip-icon.component';
import { RbacWidgetComponent } from '../../components/rbac-widget/rbac-widget.component';
import { DeleteWidgetComponent } from '../../components/delete-widget/delete-widget.component';

@Component({
  templateUrl: './view-venue.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    UnsavedChangesAlertComponent,
    AlertComponent,
    TranslateDirective,
    IsActionAllowedDirective,
    RenameButtonComponent,
    ActionButtonComponent,
    TooltipIconComponent,
    RbacWidgetComponent,
    DeleteWidgetComponent
  ]
})
export class ViewVenueComponent implements OnInit, OnDestroy, DiscardChangesDetector {
  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected venue?: VenueDto;
  protected isDirty: boolean = false;

  protected currentPage = 0;
  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.ViewVenue.Pages.Details',
      icon: 'bi-buildings'
    },
    {
      id: 1,
      title: 'Portal.ViewVenue.Pages.Settings',
      icon: 'bi-gear',
      authorization: Actions.GenericWrite
    }
  ];

  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly notificationService: NotificationService,
    private readonly modalService: NgbModal,
    private readonly titleService: TitleService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const venueId = params.get('id');
          if (venueId === null) {
            this.loadingState = { isLoading: false };
            return of();
          }
          this.loadingState = { isLoading: true };
          return this.venueService.getVenue({ id: venueId });
        })
      )
      .subscribe({
        next: (venue) => {
          this.venue = venue;
          this.titleService.setTitleFrom(venue);
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  public hasUnsavedChanges(): boolean {
    return this.isDirty;
  }

  protected togglePage(number: number): void {
    this.currentPage = number;
  }

  protected renameVenue(name: string): void {
    if (this.venue) {
      this.venue.name = name;
      this.isDirty = true;
      this.titleService.setTitleFrom(this.venue);
    }
  }

  protected editDescription(): void {
    if (!this.venue) {
      return;
    }

    const ref = this.modalService.open(TextInputDialogComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg',
      backdrop: 'static'
    });

    const component = ref.componentInstance as TextInputDialogComponent;
    component.init('Portal.ViewVenue.Details.EditDescription', this.venue.description, true, false);

    ref.closed.subscribe({
      next: (value: string) => {
        if (this.venue) {
          this.venue.description = value;
          this.isDirty = true;
        }
      }
    });
  }

  protected editTextList(which: 'addressDetails' | 'externalLinks'): void {
    if (!this.venue) {
      return;
    }

    const ref = this.modalService.open(TextListDialogComponent, {
      centered: true,
      size: 'lg',
      fullscreen: 'lg',
      backdrop: 'static'
    });

    const component = ref.componentInstance as TextListDialogComponent;

    switch (which) {
      case 'addressDetails':
        component.init('Portal.ViewVenue.Details.EditAddressDetails', undefined, this.venue.addressDetails);
        break;
      case 'externalLinks':
        component.init(
          'Portal.ViewVenue.Details.EditExternalLinks',
          /^https:\/\/(?:[A-Za-z0-9-]+\.)+[a-z]+(?:\/.*)?$/,
          this.venue.externalLinks
        );
        break;
    }

    ref.closed.subscribe({
      next: (value: string[]) => {
        if (this.venue) {
          switch (which) {
            case 'addressDetails':
              this.venue.addressDetails = value;
              break;
            case 'externalLinks':
              this.venue.externalLinks = value;
              break;
          }

          this.isDirty = true;
        }
      }
    });
  }

  protected saveChanges(): void {
    if (!this.venue || !this.isDirty || this.loadingState.isLoading) {
      return;
    }

    this.loadingState = { isLoading: true };

    this.venueService.updateVenue({ id: this.venue.id, body: this.venue }).subscribe({
      next: () => {
        this.loadingState = { isLoading: false };
        this.isDirty = false;
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected deleteVenue(): void {
    if (!this.venue) {
      return;
    }

    const organizationId = this.venue.organizationId;
    this.loadingState = { isLoading: true, error: undefined };
    this.venueService.deleteVenue({ id: this.venue.id }).subscribe({
      next: () => {
        this.notificationService.showNotification(
          'info',
          'Portal.ViewVenue.DeleteWidget.SuccessToast.Title',
          'Portal.ViewVenue.DeleteWidget.SuccessToast.Message'
        );
        void this.router.navigate([`../../organization/${organizationId}`], { relativeTo: this.route });
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }
}
