import { Routes } from '@angular/router';
import { PortalComponent } from './portal.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { AdministrationPageComponent } from './pages/administration-page/administration-page.component';
import { isAdministratorGuard } from './guards/is-administrator';
import { CreateUserComponent } from './pages/create-user/create-user.component';
import { CreateOrganizationComponent } from './pages/create-organization/create-organization.component';
import { ViewOrganizationComponent } from './pages/view-organization/view-organization.component';
import { CreateApiKeyComponent } from './pages/create-api-key/create-api-key.component';
import { CreatePlanningRealmComponent } from './pages/create-planning-realm/create-planning-realm.component';
import { CreateTournamentComponent } from './pages/create-tournament/create-tournament.component';
import { CreateVenueComponent } from './pages/create-venue/create-venue.component';
import { ViewPlanningRealmComponent } from './pages/view-planning-realm/view-planning-realm.component';
import { discardChangesGuard } from '../core/guards/discard-changes.guard';
import { FolderStatisticsComponent } from './pages/folder-statistics/folder-statistics.component';
import { FolderTimetableComponent } from './pages/folder-timetable/folder-timetable.component';
import { ViewTournamentComponent } from './pages/view-tournament/view-tournament.component';
import { ConfigureTournamentComponent } from './pages/configure-tournament/configure-tournament.component';
import { EditMatchPlanComponent } from './pages/edit-match-plan/edit-match-plan.component';
import { ViewVenueComponent } from './pages/view-venue/view-venue.component';
import { UploadImageComponent } from './pages/upload-image/upload-image.component';

export const portalRoutes: Routes = [
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
        path: 'organization/:id/upload/image',
        component: UploadImageComponent
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
