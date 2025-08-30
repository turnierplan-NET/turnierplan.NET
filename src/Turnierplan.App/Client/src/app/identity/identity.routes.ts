import { Routes } from '@angular/router';
import { IdentityComponent } from './identity.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { LoginComponent } from './pages/login/login.component';
import { ChangeUserInfoComponent } from './pages/change-user-info/change-user-info.component';

export const identityRoutes: Routes = [
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
