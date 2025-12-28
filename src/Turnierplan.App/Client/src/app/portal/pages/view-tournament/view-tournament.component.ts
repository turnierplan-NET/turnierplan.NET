import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective, TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, map, Observable, of, Subject, switchMap, takeUntil, tap } from 'rxjs';

import { NotificationService } from '../../../core/services/notification.service';
import { ComputationConfigurationComponent } from '../../components/computation-configuration/computation-configuration.component';
import { DocumentCopyComponent } from '../../components/document-copy/document-copy.component';
import { DocumentSelectComponent } from '../../components/document-select/document-select.component';
import { EditMatchComponent } from '../../components/edit-match/edit-match.component';
import { GroupsComponent, GroupTeamView, GroupView } from '../../components/groups/groups.component';
import { MatchPlanComponent, MatchView, MatchViewType } from '../../components/match-plan/match-plan.component';
import { MoveTournamentToFolderComponent } from '../../components/move-tournament-to-folder/move-tournament-to-folder.component';
import { PageFrameComponent, PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { RankingComponent, RankingView } from '../../components/ranking/ranking.component';
import { TeamListComponent, TeamView } from '../../components/team-list/team-list.component';
import { VenueSelectComponent } from '../../components/venue-select/venue-select.component';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { getDocumentName } from '../../helpers/document-name';
import { LocalStorageService } from '../../services/local-storage.service';
import { TitleService } from '../../services/title.service';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { RenameButtonComponent } from '../../components/rename-button/rename-button.component';
import { AsyncPipe, NgClass } from '@angular/common';
import { BadgeComponent } from '../../components/badge/badge.component';
import { FormsModule } from '@angular/forms';
import { LoadingIndicatorComponent } from '../../components/loading-indicator/loading-indicator.component';
import { DocumentManagerComponent } from '../../components/document-manager/document-manager.component';
import { VisibilitySelectorComponent } from '../../components/visibility-selector/visibility-selector.component';
import { AlertComponent } from '../../components/alert/alert.component';
import { ShareWidgetComponent } from '../../components/share-widget/share-widget.component';
import { PresentationConfigWidgetComponent } from '../../components/presentation-config-widget/presentation-config-widget.component';
import { ImageWidgetComponent } from '../../components/image-widget/image-widget.component';
import { RbacWidgetComponent } from '../../components/rbac-widget/rbac-widget.component';
import { DeleteWidgetComponent } from '../../components/delete-widget/delete-widget.component';
import { MatchTreeComponent } from '../../components/match-tree/match-tree.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { IdWidgetComponent } from '../../components/id-widget/id-widget.component';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { ImageType } from '../../../api/models/image-type';
import { Visibility } from '../../../api/models/visibility';
import { TournamentDto } from '../../../api/models/tournament-dto';
import { TournamentImagesDto } from '../../../api/models/tournament-images-dto';
import { DocumentDto } from '../../../api/models/document-dto';
import { getTournament } from '../../../api/fn/tournaments/get-tournament';
import { getTournamentImages } from '../../../api/fn/tournaments/get-tournament-images';
import { getDocuments } from '../../../api/fn/documents/get-documents';
import { setMatchOutcome } from '../../../api/fn/matches/set-match-outcome';
import { MatchState } from '../../../api/models/match-state';
import { DocumentType } from '../../../api/models/document-type';
import { createDocument } from '../../../api/fn/documents/create-document';
import { deleteDocument } from '../../../api/fn/documents/delete-document';
import { copyDocument } from '../../../api/fn/documents/copy-document';
import { deleteTournament } from '../../../api/fn/tournaments/delete-tournament';
import { setTeamName } from '../../../api/fn/teams/set-team-name';
import { setTeamPriority } from '../../../api/fn/teams/set-team-priority';
import { setTeamEntryFeePaid } from '../../../api/fn/teams/set-team-entry-fee-paid';
import { setTeamOutOfCompetition } from '../../../api/fn/teams/set-team-out-of-competition';
import { setGroupName } from '../../../api/fn/groups/set-group-name';
import { ComputationConfigurationDto } from '../../../api/models/computation-configuration-dto';
import { setTournamentComputationConfiguration } from '../../../api/fn/tournaments/set-tournament-computation-configuration';
import { setTournamentFolder } from '../../../api/fn/tournaments/set-tournament-folder';
import { setTournamentVenue } from '../../../api/fn/tournaments/set-tournament-venue';
import { setTournamentVisibility } from '../../../api/fn/tournaments/set-tournament-visibility';
import { SetTournamentImageEndpointRequestTarget } from '../../../api/models/set-tournament-image-endpoint-request-target';
import { setTournamentImage } from '../../../api/fn/tournaments/set-tournament-image';
import { setTournamentName } from '../../../api/fn/tournaments/set-tournament-name';
import { MatchType } from '../../../api/models/match-type';
import { MatchOutcomeType } from '../../../api/models/match-outcome-type';
import { RankingReason } from '../../../api/models/ranking-reason';
import { NewRankingOverwriteDialogComponent } from '../../components/new-ranking-overwrite-dialog/new-ranking-overwrite-dialog.component';
import { createRankingOverwrite } from '../../../api/fn/ranking-overwrites/create-ranking-overwrite';
import { CreateRankingOverwriteEndpointRequest } from '../../../api/models/create-ranking-overwrite-endpoint-request';
import { deleteRankingOverwrite } from '../../../api/fn/ranking-overwrites/delete-ranking-overwrite';

@Component({
  templateUrl: './view-tournament.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    ActionButtonComponent,
    IsActionAllowedDirective,
    SmallSpinnerComponent,
    RenameButtonComponent,
    NgClass,
    BadgeComponent,
    FormsModule,
    TranslateDirective,
    MatchPlanComponent,
    RouterLink,
    GroupsComponent,
    TeamListComponent,
    RankingComponent,
    LoadingIndicatorComponent,
    DocumentManagerComponent,
    VisibilitySelectorComponent,
    AlertComponent,
    ShareWidgetComponent,
    PresentationConfigWidgetComponent,
    ImageWidgetComponent,
    RbacWidgetComponent,
    DeleteWidgetComponent,
    MatchTreeComponent,
    AsyncPipe,
    TranslatePipe,
    TranslateDatePipe,
    IdWidgetComponent
  ]
})
export class ViewTournamentComponent implements OnInit, OnDestroy {
  public static readonly MatchPlanPageId = 0;
  public static readonly TeamListPageId = 2;
  private static readonly DocumentsPageId = 4;
  private static readonly SettingsPageId = 6;

  protected readonly ImageType = ImageType;
  protected readonly Visibility = Visibility;
  protected readonly Actions = Actions;

  protected loadingState: LoadingState = { isLoading: true };
  protected tournament?: TournamentDto;

  protected currentPage = 0;
  protected pages: PageFrameNavigationTab[] = [
    {
      id: ViewTournamentComponent.MatchPlanPageId,
      title: 'Portal.ViewTournament.Pages.MatchPlan',
      icon: 'bi-list-columns'
    },
    {
      id: 1,
      title: 'Portal.ViewTournament.Pages.Groups',
      icon: 'bi-collection'
    },
    {
      id: ViewTournamentComponent.TeamListPageId,
      title: 'Portal.ViewTournament.Pages.Teams',
      icon: 'bi-people'
    },
    {
      id: 3,
      title: 'Portal.ViewTournament.Pages.Ranking',
      icon: 'bi-list-ol'
    },
    {
      id: ViewTournamentComponent.DocumentsPageId,
      title: 'Portal.ViewTournament.Pages.Documents',
      icon: 'bi-file-earmark-ruled'
    },
    {
      id: 5,
      title: 'Portal.ViewTournament.Pages.Share',
      icon: 'bi-share'
    },
    {
      id: 6,
      title: 'Portal.ViewTournament.Pages.Settings',
      icon: 'bi-gear',
      authorization: Actions.GenericWrite
    }
  ];

  protected tournamentDate?: Date;
  protected processedMatches: MatchView[] = [];
  protected processedGroups: GroupView[] = [];
  protected processedTeams: TeamView[] = [];
  protected processedRankings: RankingView[] = [];

  protected documents?: DocumentDto[];
  protected isLoadingDocuments = false;

  protected images?: TournamentImagesDto;
  protected isLoadingImages = false;
  protected isUpdatingImage = false;

  protected canShowTournamentTree = false;
  protected showTournamentTree = false;

  protected showAccumulatedScore = false;
  protected totalScoreCount = 0;

  protected displayTeamStatistics = false;

  protected recentDocumentId?: string;

  protected isUpdatingVisibility = false;
  protected isUpdatingName = false;

  private readonly destroyed$ = new Subject<void>();

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly turnierplanApi: TurnierplanApi,
    private readonly injector: Injector,
    private readonly route: ActivatedRoute,
    private readonly titleService: TitleService,
    private readonly modalService: NgbModal,
    private readonly notificationService: NotificationService,
    private readonly router: Router,
    private readonly localStorageService: LocalStorageService,
    private readonly translateService: TranslateService
  ) { }

  protected get isTournamentTreeVisible(): boolean {
    return this.canShowTournamentTree && this.currentPage === ViewTournamentComponent.MatchPlanPageId && this.showTournamentTree;
  }

  public ngOnInit(): void {
    this.showAccumulatedScore = this.localStorageService.isDisplayAccumulateScoreEnabled();
    this.displayTeamStatistics = this.localStorageService.isDisplayTeamStatisticsEnabled();

    this.route.paramMap
      .pipe(
        takeUntil(this.destroyed$),
        switchMap((params) => {
          const tournamentId = params.get('id');
          if (tournamentId === null) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }
          this.loadingState = { isLoading: true };
          return this.turnierplanApi.invoke(getTournament, { id: tournamentId });
        })
      )
      .subscribe({
        next: (tournament) => {
          this.setTournament(tournament);
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

    // Unset the 'recentDocumentId' to prevent showing a document as recent
    // after switching to another page and then back to the documents page
    this.recentDocumentId = undefined;

    if (number === ViewTournamentComponent.DocumentsPageId && this.tournament && !this.documents && !this.isLoadingDocuments) {
      this.isLoadingDocuments = true;
      this.turnierplanApi.invoke(getDocuments, { tournamentId: this.tournament.id }).subscribe({
        next: (documents) => {
          this.documents = documents ?? [];
          this.documents.sort((a, b) => {
            const nameComparison = a.name.localeCompare(b.name);
            if (nameComparison !== 0) {
              return nameComparison;
            }
            return new Date(a.lastModifiedAt).getTime() - new Date(b.lastModifiedAt).getTime();
          });
          this.isLoadingDocuments = false;
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
    }

    if (number === ViewTournamentComponent.SettingsPageId && !this.images) {
      this.loadTournamentImages();
    }
  }

  protected loadTournamentImages(): void {
    if (!this.tournament || this.isLoadingImages) {
      return;
    }

    this.isLoadingImages = true;
    this.turnierplanApi.invoke(getTournamentImages, { id: this.tournament.id }).subscribe({
      next: (images) => {
        this.images = images;
        this.isLoadingImages = false;
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected saveAccumulatedScoreSetting(newValue: boolean): void {
    this.showAccumulatedScore = newValue;
    this.localStorageService.setDisplayAccumulateScore(newValue);
  }

  protected saveDisplayTeamStatistics(newValue: boolean): void {
    this.displayTeamStatistics = newValue;
    this.localStorageService.setDisplayTeamStatistics(newValue);
  }

  protected matchClicked(matchId: number): void {
    const tournamentId = this.tournament?.id;
    if (!tournamentId) {
      return;
    }

    const match = this.processedMatches.find((x) => x.id === matchId);

    if (match !== undefined) {
      const ref = this.modalService.open(EditMatchComponent, {
        size: 'lg',
        fullscreen: 'lg',
        centered: true,
        scrollable: true
      });
      const component = ref.componentInstance as EditMatchComponent;
      component.initialize(match, this.processedGroups);
      component.onSubmit$
        .pipe(
          tap(() => {
            const match = this.processedMatches.find((x) => x.id === matchId);
            if (match) {
              match.showLoadingIndicator = true;
            }
          }),
          switchMap((command) =>
            this.turnierplanApi
              .invoke(setMatchOutcome, { matchId: matchId, tournamentId: tournamentId, body: command })
              .pipe(map(() => command))
          ),
          switchMap((request) =>
            this.turnierplanApi.invoke(getTournament, { id: tournamentId }).pipe(
              map((tournament) => ({
                tournament: tournament,
                request: request
              }))
            )
          )
        )
        .subscribe({
          next: ({ tournament, request }) => {
            this.setTournament(tournament);
            this.notificationService.showNotification(
              'success',
              'Portal.ViewTournament.EditMatchToast.Title',
              request.state === MatchState.CurrentlyPlaying || request.state === MatchState.Finished
                ? 'Portal.ViewTournament.EditMatchToast.MessageSave'
                : 'Portal.ViewTournament.EditMatchToast.MessageClear'
            );
          },
          error: (error) => {
            this.loadingState = { isLoading: false, error: error };
          }
        });
    }
  }

  protected copyDocument(): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    const ref = this.modalService.open(DocumentCopyComponent, {
      size: 'lg',
      fullscreen: 'lg',
      centered: true,
      scrollable: true,
      injector: this.injector
    });

    const component = ref.componentInstance as DocumentCopyComponent;

    component.organization = {
      id: this.tournament.organizationId,
      name: this.tournament.organizationName
    };

    component.selected$
      .pipe(
        switchMap((documentId: string) =>
          this.turnierplanApi.invoke(copyDocument, {
            body: {
              tournamentId: tournamentId,
              sourceDocumentId: documentId
            }
          })
        ),
        tap((result) => (this.recentDocumentId = result.id)),
        switchMap(() => this.reloadDocuments()),
        finalize(() => ref.close())
      )
      .subscribe({
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });

    ref.dismissed.subscribe({
      next: (reason?: { isApiError?: boolean; apiError?: unknown }) => {
        if (reason?.isApiError === true) {
          // If reason is specified, this means an error occurred
          this.loadingState = { isLoading: false, error: reason };
        }
      }
    });
  }

  protected createDocument(): void {
    const tournamentId = this.tournament?.id;
    if (!tournamentId) {
      return;
    }

    const ref = this.modalService.open(DocumentSelectComponent, {
      size: 'lg',
      fullscreen: 'lg',
      centered: true,
      scrollable: true,
      injector: this.injector
    });

    const component = ref.componentInstance as DocumentSelectComponent;

    component.selected$
      .pipe(
        switchMap((type: DocumentType) =>
          this.turnierplanApi.invoke(createDocument, {
            body: {
              tournamentId: tournamentId,
              type: type,
              name: getDocumentName(type, this.translateService)
            }
          })
        ),
        tap((result) => (this.recentDocumentId = result.id)),
        switchMap(() => this.reloadDocuments()),
        finalize(() => ref.close())
      )
      .subscribe({
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected deleteDocument(id: string): void {
    this.turnierplanApi
      .invoke(deleteDocument, { id: id })
      .pipe(switchMap(() => this.reloadDocuments()))
      .subscribe({
        next: () => {
          this.notificationService.showNotification(
            'info',
            'Portal.ViewTournament.Documents.DeleteToast.Title',
            'Portal.ViewTournament.Documents.DeleteToast.Message'
          );
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected deleteTournament(): void {
    if (!this.tournament) {
      return;
    }
    const organizationId = this.tournament.organizationId;
    this.loadingState = { isLoading: true, error: undefined };
    this.turnierplanApi.invoke(deleteTournament, { id: this.tournament.id }).subscribe({
      next: () => {
        this.notificationService.showNotification(
          'info',
          'Portal.ViewTournament.DeleteWidget.SuccessToast.Title',
          'Portal.ViewTournament.DeleteWidget.SuccessToast.Message'
        );
        void this.router.navigate([`../../organization/${organizationId}`], { relativeTo: this.route });
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected renameTeam(teamId: number, name: string): void {
    if (!this.tournament) {
      return;
    }

    const team = this.processedTeams.find((x) => x.id === teamId);

    if (team) {
      team.showLoadingIndicator.name = true;
    }

    this.turnierplanApi.invoke(setTeamName, { teamId: teamId, tournamentId: this.tournament.id, body: { name: name } }).subscribe({
      next: () => {
        if (this.tournament) {
          const team = this.tournament.teams.find((x) => x.id === teamId);

          if (team) {
            team.name = name;
          }

          this.processTournament();
        }
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected setTeamPriority(teamId: number, groupId: number, priority: number): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;
    const team = this.processedTeams.find((x) => x.id === teamId);

    if (team) {
      team.showLoadingIndicator.priority = true;
    }

    this.turnierplanApi
      .invoke(setTeamPriority, { teamId: teamId, tournamentId: this.tournament.id, body: { groupId: groupId, priority: priority } })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId })))
      .subscribe({
        next: (result) => {
          // The tournament must be re-loaded because changing the priority of any team might change group/match outcomes.
          this.setTournament(result);
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected setTeamEntryFeePaid(teamId: number, entryFeePaid: boolean): void {
    if (!this.tournament) {
      return;
    }

    const team = this.processedTeams.find((x) => x.id === teamId);

    if (team) {
      team.showLoadingIndicator.entryFee = true;
    }

    this.turnierplanApi
      .invoke(setTeamEntryFeePaid, { teamId: teamId, tournamentId: this.tournament.id, body: { hasPaidEntryFee: entryFeePaid } })
      .subscribe({
        next: () => {
          if (this.tournament) {
            const team = this.tournament.teams.find((x) => x.id === teamId);

            if (team) {
              if (entryFeePaid) {
                // This is only an approximation because the actual value of the 'entryFeePaidAt' of the team will
                // be slightly different. IDEA: An additional request could be performed to get the actual value.
                team.entryFeePaidAt = new Date().toISOString();
              } else {
                team.entryFeePaidAt = undefined;
              }
            }

            this.processTournament();
          }
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected setTeamOutOfCompetition(teamId: number, outOfCompetition: boolean): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;
    const team = this.processedTeams.find((x) => x.id === teamId);

    if (team) {
      team.showLoadingIndicator.outOfCompetition = true;
    }

    this.turnierplanApi
      .invoke(setTeamOutOfCompetition, { teamId: teamId, tournamentId: this.tournament.id, body: { outOfCompetition: outOfCompetition } })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId })))
      .subscribe({
        next: (result) => {
          // The tournament must be re-loaded because changing 'out of competition' of any team might change group/match outcomes.
          this.setTournament(result);
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected renameGroup(groupId: number, name?: string): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    const group = this.processedGroups.find((x) => x.id === groupId);

    if (group) {
      group.showLoadingIndicator = true;
    }

    this.turnierplanApi
      .invoke(setGroupName, { groupId: groupId, tournamentId: this.tournament.id, body: { name: name } })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId })))
      .subscribe({
        next: (tournament) => {
          this.setTournament(tournament);
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected editComputationConfiguration(): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    const ref = this.modalService.open(ComputationConfigurationComponent, {
      size: 'lg',
      fullscreen: 'lg',
      centered: true
    });
    const component = ref.componentInstance as ComputationConfigurationComponent;
    component.initialize(this.tournament.computationConfiguration);

    component.save$
      .pipe(
        switchMap((configuration: ComputationConfigurationDto) => {
          return this.turnierplanApi
            .invoke(setTournamentComputationConfiguration, { id: tournamentId, body: { configuration: configuration } })
            .pipe(map(() => configuration));
        }),
        finalize(() => ref.close()),
        switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId }))
      )
      .subscribe({
        next: (tournament) => {
          this.setTournament(tournament);
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected moveToAnotherFolder(): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    const ref = this.modalService.open(MoveTournamentToFolderComponent, {
      injector: this.injector,
      size: 'lg',
      fullscreen: 'lg',
      centered: true
    });
    const component = ref.componentInstance as MoveTournamentToFolderComponent;

    component.initialize(this.tournament.organizationId, this.tournament.folderId, this.tournament.folderName ?? undefined);

    component.save$
      .pipe(
        switchMap((command) => {
          return this.turnierplanApi.invoke(setTournamentFolder, { id: tournamentId, body: command });
        }),
        switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId })),
        finalize(() => ref.close())
      )
      .subscribe({
        next: (tournament) => {
          this.setTournament(tournament);
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected editVenueAssignment(): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    const ref = this.modalService.open(VenueSelectComponent, {
      injector: this.injector,
      size: 'lg',
      fullscreen: 'lg',
      centered: true
    });
    const component = ref.componentInstance as VenueSelectComponent;
    component.initialize(this.tournament.organizationId, this.tournament.venueId);

    component.selected$
      .pipe(
        switchMap((venueData) => {
          return this.turnierplanApi
            .invoke(setTournamentVenue, { id: tournamentId, body: { venueId: venueData?.id } })
            .pipe(map(() => venueData));
        }),
        finalize(() => ref.close())
      )
      .subscribe({
        next: (venueData) => {
          if (this.tournament) {
            this.tournament.venueId = venueData?.id;
            this.tournament.venueName = venueData?.name;
          }
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected setTournamentVisibility(visibility: Visibility): void {
    if (!this.tournament || this.tournament.visibility === visibility) {
      return;
    }

    this.isUpdatingVisibility = true;

    this.turnierplanApi.invoke(setTournamentVisibility, { id: this.tournament.id, body: { visibility: visibility } }).subscribe({
      next: () => {
        if (this.tournament) {
          this.tournament.visibility = visibility;
        }
        this.isUpdatingVisibility = false;
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected setImage(whichImage: 'primaryLogo' | 'secondaryLogo' | 'bannerImage', imageId?: string): void {
    if (!this.tournament || !this.images || this.isUpdatingImage) {
      return;
    }

    if (this.images[whichImage]?.id === imageId) {
      return;
    }

    this.isUpdatingImage = true;

    const tournamentId = this.tournament.id;

    const mappedTarget = {
      primaryLogo: SetTournamentImageEndpointRequestTarget.PrimaryLogo,
      secondaryLogo: SetTournamentImageEndpointRequestTarget.SecondaryLogo,
      bannerImage: SetTournamentImageEndpointRequestTarget.BannerImage
    }[whichImage];

    this.turnierplanApi
      .invoke(setTournamentImage, { id: tournamentId, body: { imageId: imageId, target: mappedTarget } })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getTournamentImages, { id: tournamentId })))
      .subscribe({
        next: (result) => {
          this.images = result;
          this.isUpdatingImage = false;
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected renameTournament(name: string): void {
    if (!this.tournament || name === this.tournament.name || this.isUpdatingName) {
      return;
    }

    this.isUpdatingName = true;

    this.turnierplanApi.invoke(setTournamentName, { id: this.tournament.id, body: { name: name } }).subscribe({
      next: () => {
        if (this.tournament) {
          this.tournament.name = name;
          this.titleService.setTitleFrom(this.tournament);
        }
        this.isUpdatingName = false;
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected addRankingOverwrite(): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    const ref = this.modalService.open(NewRankingOverwriteDialogComponent, {
      size: 'md',
      fullscreen: 'md',
      centered: true,
      scrollable: true
    });

    const component = ref.componentInstance as NewRankingOverwriteDialogComponent;
    component.teams = this.processedTeams;
    component.existingOverwrites = this.tournament.rankingOverwrites;

    ref.closed
      .pipe(
        tap(() => (this.loadingState = { isLoading: true })),
        switchMap((request: CreateRankingOverwriteEndpointRequest) =>
          this.turnierplanApi.invoke(createRankingOverwrite, { id: tournamentId, body: request })
        ),
        switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId }))
      )
      .subscribe({
        next: (tournament): void => {
          this.setTournament(tournament);
          this.loadingState = { isLoading: false };

          this.notificationService.showNotification(
            'success',
            'Portal.ViewTournament.AddRankingOverwriteToast.Title',
            'Portal.ViewTournament.AddRankingOverwriteToast.Message'
          );
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected deleteRankingOverwrite(id: number): void {
    if (!this.tournament) {
      return;
    }

    const tournamentId = this.tournament.id;

    this.turnierplanApi
      .invoke(deleteRankingOverwrite, { tournamentId: tournamentId, rankingOverwriteId: id })
      .pipe(
        tap(() => (this.loadingState = { isLoading: true })),
        switchMap(() => this.turnierplanApi.invoke(getTournament, { id: tournamentId }))
      )
      .subscribe({
        next: (tournament): void => {
          this.setTournament(tournament);
          this.loadingState = { isLoading: false };

          this.notificationService.showNotification(
            'info',
            'Portal.ViewTournament.DeleteRankingOverwriteToast.Title',
            'Portal.ViewTournament.DeleteRankingOverwriteToast.Message'
          );
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  private reloadDocuments(): Observable<unknown> {
    if (!this.tournament) {
      return of({});
    }
    return this.turnierplanApi.invoke(getDocuments, { tournamentId: this.tournament.id }).pipe(
      tap((result) => {
        this.documents = result;
        this.documents.sort((a, b) => {
          const nameComparison = a.name.localeCompare(b.name);
          if (nameComparison !== 0) {
            return nameComparison;
          }
          return new Date(a.lastModifiedAt).getTime() - new Date(b.lastModifiedAt).getTime();
        });
      })
    );
  }

  private setTournament(tournament: TournamentDto | undefined): void {
    this.tournament = tournament;
    this.titleService.setTitleFrom(tournament);
    this.processTournament();
  }

  private processTournament(): void {
    if (!this.tournament) {
      this.tournamentDate = undefined;
      this.processedMatches = [];
      this.canShowTournamentTree = false;
      this.processedGroups = [];
      this.processedTeams = [];
      this.totalScoreCount = 0;
      return;
    }

    const matchKickoffs = this.tournament.matches.filter((x) => x.kickoff !== undefined).map((x) => new Date(x.kickoff!));
    if (matchKickoffs.length > 0) {
      matchKickoffs.sort((a, b) => a.getTime() - b.getTime());
      this.tournamentDate = matchKickoffs[0];
    }

    let hasProcessedNonGroupMatches = false;

    this.totalScoreCount = 0;
    this.processedMatches = this.tournament.matches.map((match): MatchView => {
      let groupName = '';
      if (match.type === MatchType.GroupMatch) {
        const matchGroupId = match.groupId;
        const group = this.tournament?.groups.find((group) => group.id === match.groupId);
        groupName = group?.alphabeticalId ?? (matchGroupId ? `${matchGroupId}` : '');
      }

      const type: MatchViewType = {
        matchType: match.type,
        displayName: match.formattedType,
        playoffPosition: match.playoffPosition ?? undefined,
        hideOnMatchPlan: false
      };

      if (match.type === MatchType.GroupMatch) {
        type.hideOnMatchPlan = !hasProcessedNonGroupMatches;
      } else {
        hasProcessedNonGroupMatches = true;
      }

      const includeInAccumulatedScore =
        (match.state === MatchState.CurrentlyPlaying || match.state === MatchState.Finished) &&
        match.outcomeType !== MatchOutcomeType.SpecialScoring;

      if (includeInAccumulatedScore) {
        this.totalScoreCount += match.teamA?.score ?? 0;
        this.totalScoreCount += match.teamB?.score ?? 0;
      }

      return {
        id: match.id,
        index: match.index,
        court: `${match.court + 1}`,
        type: type,
        kickoff: match.kickoff ? new Date(match.kickoff) : undefined,
        group: groupName,
        teamA: this.tournament?.teams.find((team) => team.id === match.teamA.teamId)?.name ?? '',
        teamB: this.tournament?.teams.find((team) => team.id === match.teamB.teamId)?.name ?? '',
        teamSelectorA:
          match.type === MatchType.GroupMatch || match.teamA.teamSelector.key.startsWith('T') ? '' : match.teamA.teamSelector.localized,
        teamSelectorB:
          match.type === MatchType.GroupMatch || match.teamB.teamSelector.key.startsWith('T') ? '' : match.teamB.teamSelector.localized,
        teamSelectorKeyA: match.teamA.teamSelector.key,
        teamSelectorKeyB: match.teamB.teamSelector.key,
        isLive: match.state === MatchState.CurrentlyPlaying,
        scoreA: match.teamA?.score ?? undefined,
        scoreB: match.teamB?.score ?? undefined,
        outcomeType: match.outcomeType ? (match.outcomeType === MatchOutcomeType.Null ? undefined : match.outcomeType) : undefined,
        scoreAccumulated: includeInAccumulatedScore ? this.totalScoreCount : undefined,
        showLoadingIndicator: false
      };
    });

    this.canShowTournamentTree = this.processedMatches.filter((match) => match.type.matchType === MatchType.Final).length === 1;

    this.processedGroups = this.tournament.groups.map((group): GroupView => {
      const mappedTeams: GroupTeamView[] = group.participants.map((assignment): GroupTeamView => {
        const team = this.tournament?.teams.find((team) => team.id === assignment.teamId);
        return {
          id: assignment.teamId,
          position: assignment.statistics?.position ?? group.participants.length,
          name: team?.name ?? `${assignment.teamId}`,
          matches: assignment.statistics?.matchesPlayed ?? 0,
          won: assignment.statistics?.matchesWon ?? 0,
          drawn: assignment.statistics?.matchesDrawn ?? 0,
          lost: assignment.statistics?.matchesLost ?? 0,
          score: `${assignment.statistics?.scoreFor ?? 0}:${assignment.statistics?.scoreAgainst ?? 0}`,
          scoreDiff: assignment.statistics?.scoreDifference ?? 0,
          points: assignment.statistics?.points ?? 0
        };
      });

      mappedTeams.sort((a, b) => {
        if (a.position === b.position) {
          return a.name.localeCompare(b.name);
        } else {
          return a.position - b.position;
        }
      });

      return {
        id: group.id,
        displayName: group.displayName,
        hasCustomDisplayName: group.hasCustomDisplayName,
        alphabeticalId: group.alphabeticalId,
        teams: mappedTeams,
        showLoadingIndicator: false
      };
    });

    this.processedTeams = this.tournament.teams.map((team) => {
      const convertedTeam: TeamView = {
        id: team.id,
        name: team.name,
        outOfCompetition: team.outOfCompetition,
        entryFeePaidAt: team.entryFeePaidAt ? new Date(team.entryFeePaidAt) : undefined,
        groupId: undefined,
        priority: undefined,
        matches: team.statistics.matchesPlayed,
        won: team.statistics.matchesWon,
        drawn: team.statistics.matchesDrawn,
        lost: team.statistics.matchesLost,
        score: `${team.statistics.scoreFor}:${team.statistics.scoreAgainst}`,
        teamLink: team.link,
        showLoadingIndicator: {
          name: false,
          priority: false,
          entryFee: false,
          outOfCompetition: false
        }
      };

      const groups = (this.tournament?.groups ?? []).filter((group) =>
        group.participants.some((participant) => participant.teamId === team.id)
      );

      // IDEA: Add support for teams being assigned to multiple groups
      if (groups.length > 0) {
        convertedTeam.groupId = groups[0].id;
        convertedTeam.priority = groups[0].participants.find((x) => x.teamId === team.id)?.priority;
      }

      return convertedTeam;
    });

    this.processedRankings = [];
    this.processedRankings.push(
      ...this.tournament.rankings.map((ranking) => {
        const teamName = ranking.teamId ? this.tournament?.teams.find((team) => team.id == ranking.teamId)?.name : undefined;

        // Finding the ranking overwrite by teamId & placementRank is a "best guess" because there could be multiple overwrites satisfying this
        // condition. However, in that case deleting either one by its id would result in the same outcome. So for the moment, this is sufficient.
        const rankingOverwriteId =
          ranking.reason === RankingReason.ManuallyChanged
            ? this.tournament?.rankingOverwrites.find((o) => o.placementRank === ranking.placementRank && o.assignTeamId === ranking.teamId)
              ?.id
            : undefined;

        return {
          position: ranking.placementRank,
          isHidden: false,
          rankingOverwriteId: rankingOverwriteId,
          team: teamName,
          reason: ranking.reason
        };
      })
    );
    // "Hidden" rankings don't appear in the ranking position array. Because of this, we need
    // to construct these ranking views from the overwrites which cause them to be hidden.
    this.processedRankings.push(
      ...this.tournament.rankingOverwrites
        .filter((overwrite) => overwrite.hideRanking)
        .map((overwrite) => {
          return {
            position: overwrite.placementRank,
            isHidden: true,
            rankingOverwriteId: overwrite.id,
            team: undefined,
            reason: undefined
          };
        })
    );
    this.processedRankings.sort((a, b) => a.position - b.position);
  }
}
