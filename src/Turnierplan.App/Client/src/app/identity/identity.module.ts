import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { SharedModule } from '../shared/shared.module';

import { IdentityComponent } from './identity.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { ChangeUserInfoComponent } from './pages/change-user-info/change-user-info.component';
import { LoginComponent } from './pages/login/login.component';

const routes: Routes = [
  {
    path: '',
    component: IdentityComponent,
    children: [
      {
        path: 'change-password',
        component: ChangePasswordComponent
      },
      {
        path: 'login',
        component: LoginComponent
      },
      {
        path: 'user-info',
        component: ChangeUserInfoComponent
      }
    ]
  }
];

export const identityPages = routes[0].children!.map((x) => x.path);

@NgModule({
  declarations: [IdentityComponent, ChangePasswordComponent, ChangeUserInfoComponent, LoginComponent],
  imports: [CommonModule, RouterModule.forChild(routes), TranslateModule.forChild(), SharedModule, FormsModule, ReactiveFormsModule]
})
export class IdentityModule {}
