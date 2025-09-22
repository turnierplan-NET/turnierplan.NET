import { Routes } from '@angular/router';
import { IdentityComponent } from './identity.component';
import { ChangePasswordComponent } from './pages/change-password/change-password.component';
import { LoginComponent } from './pages/login/login.component';

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
      }
    ]
  }
];
