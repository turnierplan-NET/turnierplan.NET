import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { mergeMap, Observable, of, Subject, switchMap, takeUntil, tap, zip } from 'rxjs';

import {
  ApiKeyDto,
  OrganizationDto,
  TournamentHeaderDto,
  VenueDto,
  ApiKeysService,
  OrganizationsService,
  TournamentsService,
  VenuesService,
  PlanningRealmHeaderDto,
  PlanningRealmsService
} from '../../../api';
import { NotificationService } from '../../../core/services/notification.service';
import { PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';

@Component({
  standalone: false,
  templateUrl: './view-organization.component.html'
})
export class ViewOrganizationComponent implements OnInit, OnDestroy {
  private static readonly venuesPageId = 1;
  private static readonly apiKeysPageId = 2;
  private static readonly planningRealmsPageId = 4;

  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected organization?: OrganizationDto;
  protected tournaments?: TournamentHeaderDto[];
  protected venues?: VenueDto[];
  protected planningRealms?: PlanningRealmHeaderDto[];
  protected apiKeys?: ApiKeyDto[];
  protected displayApiKeyUsage?: string;
  protected isLoadingVenues = false;
  protected isLoadingApiKeys = false;
  protected isLoadingPlanningRealms = false;

  protected isUpdatingName = false;

  protected currentPage = 0;
  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.ViewOrganization.Pages.Tournaments',
      icon: 'bi-trophy'
    },
    {
      id: ViewOrganizationComponent.venuesPageId,
      title: 'Portal.ViewOrganization.Pages.Venues',
      icon: 'bi-buildings'
    },
    {
      id: ViewOrganizationComponent.planningRealmsPageId,
      title: 'Portal.ViewOrganization.Pages.PlanningRealms',
      icon: 'bi-ticket-perforated'
    },
    {
      id: ViewOrganizationComponent.apiKeysPageId,
      title: 'Portal.ViewOrganization.Pages.ApiKeys',
      icon: 'bi-key'
    },
    {
      id: 3,
      title: 'Portal.ViewOrganization.Pages.Settings',
      icon: 'bi-gear',
      authorization: Actions.GenericWrite
    }
  ];

  private readonly destroyed$ = new Subject<void>();

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly route: ActivatedRoute,
    private readonly organizationService: OrganizationsService,
    private readonly tournamentService: TournamentsService,
    private readonly venueService: VenuesService,
    private readonly planningRealmsService: PlanningRealmsService,
    private readonly apiKeyService: ApiKeysService,
    private readonly titleService: TitleService,
    private readonly router: Router,
    private readonly notificationService: NotificationService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const organizationId = params.get('id');
          if (organizationId === null) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }
          this.loadingState = { isLoading: true };
          return this.organizationService.getOrganization({ id: organizationId });
        }),
        mergeMap((organization) => {
          if (!organization) {
            return of([undefined, undefined]);
          }

          return zip(of(organization), this.tournamentService.getTournaments({ organizationId: organization.id }));
        })
      )
      .subscribe({
        next: (result) => {
          this.organization = result[0];
          this.tournaments = result[1];

          this.titleService.setTitleFrom(this.organization);

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

  protected togglePage(number: number): void {
    this.currentPage = number;

    if (!this.organization) {
      return;
    }

    if (number === ViewOrganizationComponent.venuesPageId && !this.venues && !this.isLoadingVenues) {
      // Load venues only when the page is opened
      this.isLoadingVenues = true;
      this.venueService.getVenues({ organizationId: this.organization.id }).subscribe({
        next: (venues) => {
          this.venues = venues;
          this.isLoadingVenues = false;
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
    }

    if (number === ViewOrganizationComponent.planningRealmsPageId && !this.planningRealms && !this.isLoadingPlanningRealms) {
      // Load planning realms only when the page is opened
      this.isLoadingPlanningRealms = true;
      this.planningRealmsService.getPlanningRealms({ organizationId: this.organization.id }).subscribe({
        next: (planningRealms) => {
          this.planningRealms = planningRealms;
          this.isLoadingPlanningRealms = false;
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
    }

    if (number === ViewOrganizationComponent.apiKeysPageId && !this.apiKeys && !this.isLoadingApiKeys) {
      // Load API keys only when the page is opened
      this.loadApiKeys().subscribe({
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
    }
  }

  protected deleteOrganization(): void {
    if (!this.organization) {
      return;
    }
    this.loadingState = { isLoading: true, error: undefined };
    this.organizationService.deleteOrganization({ id: this.organization.id }).subscribe({
      next: () => {
        this.notificationService.showNotification(
          'info',
          'Portal.ViewOrganization.DeleteWidget.SuccessToast.Title',
          'Portal.ViewOrganization.DeleteWidget.SuccessToast.Message'
        );
        void this.router.navigate(['..'], { relativeTo: this.route });
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected renameOrganization(name: string): void {
    if (!this.organization || name === this.organization.name || this.isUpdatingName) {
      return;
    }

    this.isUpdatingName = true;

    this.organizationService.setOrganizationName({ id: this.organization.id, body: { name: name } }).subscribe({
      next: () => {
        if (this.organization) {
          this.organization.name = name;
        }
        this.isUpdatingName = false;
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected deleteApiKey(id: string): void {
    this.apiKeyService
      .deleteApiKey({ id: id })
      .pipe(switchMap(() => this.loadApiKeys()))
      .subscribe({
        next: () => {
          this.notificationService.showNotification(
            'info',
            'Portal.ViewOrganization.ApiKeys.DeleteToast.Title',
            'Portal.ViewOrganization.ApiKeys.DeleteToast.Message'
          );
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected setApiKeyActive(apiKey: ApiKeyDto, isActive: boolean): void {
    if (apiKey.isExpired) {
      return;
    }

    this.apiKeyService
      .setApiKeyStatus({ id: apiKey.id, body: { isActive: isActive } })
      .pipe(switchMap(() => this.loadApiKeys()))
      .subscribe({
        next: () => {
          if (!isActive) {
            this.displayApiKeyUsage = undefined;
          }
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  private loadApiKeys(): Observable<unknown> {
    if (!this.organization) {
      return of({});
    }

    this.isLoadingApiKeys = true;

    return this.apiKeyService.getApiKeys({ organizationId: this.organization.id }).pipe(
      tap((apiKeys) => {
        this.apiKeys = apiKeys;
        this.isLoadingApiKeys = false;
      })
    );
  }
}
