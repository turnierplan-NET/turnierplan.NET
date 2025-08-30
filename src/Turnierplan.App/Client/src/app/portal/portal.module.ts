import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import {
  NgbDropdown,
  NgbDropdownItem,
  NgbDropdownMenu,
  NgbDropdownToggle,
  NgbPagination,
  NgbPopover,
  NgbTooltip
} from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { PdfJsViewerModule } from 'ng2-pdfjs-viewer';
import { DndDraggableDirective, DndDropzoneDirective, DndHandleDirective, DndPlaceholderRefDirective } from 'ngx-drag-drop';

import { discardChangesGuard } from '../core/guards/discard-changes.guard';
import { SharedModule } from '../shared/shared.module';

import { ActionButtonComponent } from './components/action-button/action-button.component';
import { AlertComponent } from './components/alert/alert.component';
import { ApiKeyUsageComponent } from './components/api-key-usage/api-key-usage.component';
import { BadgeComponent } from './components/badge/badge.component';
import { ComputationConfigurationComponent } from './components/computation-configuration/computation-configuration.component';
import { CopyToClipboardComponent } from './components/copy-to-clipboard/copy-to-clipboard.component';
import { DeleteButtonComponent } from './components/delete-button/delete-button.component';
import { DeleteWidgetComponent } from './components/delete-widget/delete-widget.component';
import { DocumentConfigFrameComponent } from './components/document-config-frame/document-config-frame.component';
import { DocumentConfigMatchPlanComponent } from './components/document-config-match-plan/document-config-match-plan.component';
import { DocumentConfigReceiptsComponent } from './components/document-config-receipts/document-config-receipts.component';
import { DocumentCopyComponent } from './components/document-copy/document-copy.component';
import { DocumentManagerComponent } from './components/document-manager/document-manager.component';
import { DocumentSelectComponent } from './components/document-select/document-select.component';
import { DurationPickerComponent } from './components/duration-picker/duration-picker.component';
import { EditMatchComponent } from './components/edit-match/edit-match.component';
import { ErrorPageComponent } from './components/error-page/error-page.component';
import { FolderTreeComponent } from './components/folder-tree/folder-tree.component';
import { GroupsComponent } from './components/groups/groups.component';
import { IllustrationComponent } from './components/illustration/illustration.component';
import { ImageChooserComponent } from './components/image-chooser/image-chooser.component';
import { ImageWidgetComponent } from './components/image-widget/image-widget.component';
import { LiveIndicatorComponent } from './components/live-indicator/live-indicator.component';
import { LoadingErrorComponent } from './components/loading-error/loading-error.component';
import { LoadingIndicatorComponent } from './components/loading-indicator/loading-indicator.component';
import { MatchPlanComponent } from './components/match-plan/match-plan.component';
import { MatchTreeComponent } from './components/match-tree/match-tree.component';
import { MoveTournamentToFolderComponent } from './components/move-tournament-to-folder/move-tournament-to-folder.component';
import { PageFrameComponent } from './components/page-frame/page-frame.component';
import { PresentationConfigWidgetComponent } from './components/presentation-config-widget/presentation-config-widget.component';
import { RankingComponent } from './components/ranking/ranking.component';
import { RenameButtonComponent } from './components/rename-button/rename-button.component';
import { RenameDialogComponent } from './components/rename-dialog/rename-dialog.component';
import { ShareWidgetComponent } from './components/share-widget/share-widget.component';
import { TeamListComponent } from './components/team-list/team-list.component';
import { TextInputDialogComponent } from './components/text-input-dialog/text-input-dialog.component';
import { TextListDialogComponent } from './components/text-list-dialog/text-list-dialog.component';
import { TooltipIconComponent } from './components/tooltip-icon/tooltip-icon.component';
import { TournamentEditWarningComponent } from './components/tournament-edit-warning/tournament-edit-warning.component';
import { TournamentExplorerComponent } from './components/tournament-explorer/tournament-explorer.component';
import { TournamentSelectComponent } from './components/tournament-select/tournament-select.component';
import { ValidationErrorDialogComponent } from './components/validation-error-dialog/validation-error-dialog.component';
import { VenueSelectComponent } from './components/venue-select/venue-select.component';
import { VenueTileComponent } from './components/venue-tile/venue-tile.component';
import { VisibilitySelectorComponent } from './components/visibility-selector/visibility-selector.component';
import { IsAdministratorDirective } from './directives/is-administrator/is-administrator.directive';
import { LoadingStateDirective } from './directives/loading-state/loading-state.directive';
import { isAdministratorGuard } from './guards/is-administrator';
import { AdministrationPageComponent } from './pages/administration-page/administration-page.component';
import { ConfigureTournamentComponent } from './pages/configure-tournament/configure-tournament.component';
import { CreateApiKeyComponent } from './pages/create-api-key/create-api-key.component';
import { CreateOrganizationComponent } from './pages/create-organization/create-organization.component';
import { CreateTournamentComponent } from './pages/create-tournament/create-tournament.component';
import { CreateUserComponent } from './pages/create-user/create-user.component';
import { CreateVenueComponent } from './pages/create-venue/create-venue.component';
import { EditMatchPlanComponent } from './pages/edit-match-plan/edit-match-plan.component';
import { FolderStatisticsComponent } from './pages/folder-statistics/folder-statistics.component';
import { FolderTimetableComponent } from './pages/folder-timetable/folder-timetable.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { ViewOrganizationComponent } from './pages/view-organization/view-organization.component';
import { ViewTournamentComponent } from './pages/view-tournament/view-tournament.component';
import { ViewVenueComponent } from './pages/view-venue/view-venue.component';
import { AbstractTeamSelectorPipe } from './pipes/abstract-team-selector.pipe';
import { TranslateDatePipe } from './pipes/translate-date.pipe';
import { PortalComponent } from './portal.component';
import { LocalStorageService } from './services/local-storage.service';
import { TitleService } from './services/title.service';
import { QRCodeComponent } from 'angularx-qrcode';
import { RbacWidgetComponent } from './components/rbac-widget/rbac-widget.component';
import { RbacOffcanvasComponent } from './components/rbac-offcanvas/rbac-offcanvas.component';
import { RbacPrincipalComponent } from './components/rbac-principal/rbac-principal.component';
import { RbacAddAssignmentComponent } from './components/rbac-add-assignment/rbac-add-assignment.component';
import { IsActionAllowedDirective } from './directives/is-action-allowed/is-action-allowed.directive';
import { CreatePlanningRealmComponent } from './pages/create-planning-realm/create-planning-realm.component';
import { ViewPlanningRealmComponent } from './pages/view-planning-realm/view-planning-realm.component';
import { TournamentClassManagerComponent } from './components/tournament-class-manager/tournament-class-manager.component';
import { InvitationLinkTileComponent } from './components/invitation-link-manager/invitation-link-tile.component';
import { UnsavedChangesAlertComponent } from './components/unsaved-changes-alert/unsaved-changes-alert.component';
import { ShareLinkComponent } from './components/share-link/share-link.component';
import { ManageApplicationsComponent } from './components/manage-applications/manage-applications.component';
import { ManageApplicationsFilterComponent } from './components/manage-applications-filter/manage-applications-filter.component';
import { MultiSelectFilterComponent } from './components/multi-select-filter/multi-select-filter.component';
import { NewApplicationDialogComponent } from './components/new-application-dialog/new-application-dialog.component';

const routes: Routes = [
  {
    path: '',
    component: PortalComponent,
    children: [
      {
        path: '',
        component: LandingPageComponent
      },
      {
        path: 'administration',
        component: AdministrationPageComponent,
        canActivate: [isAdministratorGuard()]
      },
      {
        path: 'administration/create/user',
        component: CreateUserComponent,
        canActivate: [isAdministratorGuard()]
      },
      {
        path: 'create/organization',
        component: CreateOrganizationComponent
      },
      {
        path: 'organization/:id',
        component: ViewOrganizationComponent
      },
      {
        path: 'organization/:id/create/api-key',
        component: CreateApiKeyComponent
      },
      {
        path: 'organization/:id/create/planning-realm',
        component: CreatePlanningRealmComponent
      },
      {
        path: 'organization/:id/create/tournament',
        component: CreateTournamentComponent
      },
      {
        path: 'organization/:id/create/venue',
        component: CreateVenueComponent
      },
      {
        path: 'planning-realm/:id',
        component: ViewPlanningRealmComponent,
        canDeactivate: [discardChangesGuard]
      },
      {
        path: 'statistics/:id',
        component: FolderStatisticsComponent
      },
      {
        path: 'timetable/:id',
        component: FolderTimetableComponent
      },
      {
        path: 'tournament/:id',
        component: ViewTournamentComponent
      },
      {
        path: 'tournament/:id/configure',
        component: ConfigureTournamentComponent,
        canDeactivate: [discardChangesGuard]
      },
      {
        path: 'tournament/:id/edit-match-plan',
        component: EditMatchPlanComponent,
        canDeactivate: [discardChangesGuard]
      },
      {
        path: 'venue/:id',
        component: ViewVenueComponent,
        canDeactivate: [discardChangesGuard]
      },
      {
        path: '**',
        redirectTo: '/portal'
      }
    ]
  }
];

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forChild(routes),
        FormsModule,
        ReactiveFormsModule,
        TranslateModule.forChild(),
        SharedModule,
        NgbPopover,
        DndDropzoneDirective,
        DndDraggableDirective,
        DndPlaceholderRefDirective,
        PdfJsViewerModule,
        NgbTooltip,
        TranslateModule,
        DndHandleDirective,
        QRCodeComponent,
        NgbDropdown,
        NgbDropdownItem,
        NgbDropdownMenu,
        NgbDropdownToggle,
        NgbPagination,
        PortalComponent,
        LandingPageComponent,
        CreateOrganizationComponent,
        ViewOrganizationComponent,
        IllustrationComponent,
        ErrorPageComponent,
        LoadingStateDirective,
        TranslateDatePipe,
        ViewTournamentComponent,
        CreateTournamentComponent,
        ActionButtonComponent,
        PageFrameComponent,
        GroupsComponent,
        MatchPlanComponent,
        RankingComponent,
        TeamListComponent,
        LoadingErrorComponent,
        EditMatchComponent,
        DeleteWidgetComponent,
        ConfigureTournamentComponent,
        DeleteButtonComponent,
        DurationPickerComponent,
        DocumentManagerComponent,
        DocumentSelectComponent,
        DocumentConfigFrameComponent,
        DocumentConfigReceiptsComponent,
        RenameButtonComponent,
        RenameDialogComponent,
        AlertComponent,
        AbstractTeamSelectorPipe,
        ComputationConfigurationComponent,
        CreateVenueComponent,
        VenueTileComponent,
        ViewVenueComponent,
        VenueSelectComponent,
        ShareWidgetComponent,
        CopyToClipboardComponent,
        TournamentExplorerComponent,
        MoveTournamentToFolderComponent,
        TooltipIconComponent,
        DocumentConfigMatchPlanComponent,
        VisibilitySelectorComponent,
        LoadingIndicatorComponent,
        LiveIndicatorComponent,
        ImageWidgetComponent,
        ImageChooserComponent,
        TextInputDialogComponent,
        TextListDialogComponent,
        EditMatchPlanComponent,
        TournamentEditWarningComponent,
        FolderTimetableComponent,
        CreateApiKeyComponent,
        ApiKeyUsageComponent,
        DocumentCopyComponent,
        FolderTreeComponent,
        ValidationErrorDialogComponent,
        PresentationConfigWidgetComponent,
        TournamentSelectComponent,
        MatchTreeComponent,
        FolderStatisticsComponent,
        IsAdministratorDirective,
        AdministrationPageComponent,
        CreateUserComponent,
        BadgeComponent,
        RbacWidgetComponent,
        RbacOffcanvasComponent,
        RbacAddAssignmentComponent,
        RbacPrincipalComponent,
        IsActionAllowedDirective,
        CreatePlanningRealmComponent,
        ViewPlanningRealmComponent,
        TournamentClassManagerComponent,
        InvitationLinkTileComponent,
        UnsavedChangesAlertComponent,
        ShareLinkComponent,
        ManageApplicationsComponent,
        ManageApplicationsFilterComponent,
        MultiSelectFilterComponent,
        NewApplicationDialogComponent
    ],
    providers: [LocalStorageService, TitleService]
})
export class PortalModule {}
