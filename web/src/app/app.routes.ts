import { Routes } from '@angular/router';
import { Component } from '@angular/core';

@Component({
  template: ' Startseite'
})
class TempHome {}

@Component({
  template: ' Organisationen'
})
class TempOrganizations {}

@Component({
  template: ' Turniere'
})
class TempTournaments {}

export const routes: Routes = [
  {
    path: 'portal-v2',
    children: [
      { path: '', component: TempHome },
      { path: 'organizations', component: TempOrganizations },
      { path: 'tournaments', component: TempTournaments }
    ]
  },
  {
    path: '**',
    pathMatch: 'full',
    redirectTo: '/portal-v2'
  }
];
