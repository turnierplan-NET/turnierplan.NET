import { CommonModule } from '@angular/common';
import { ModuleWithProviders, NgModule } from '@angular/core';

import { AuthenticationService } from './services/authentication.service';
import { NotificationService } from './services/notification.service';
import { AuthorizationService } from './services/authorization.service';

@NgModule({
  declarations: [],
  imports: [CommonModule]
})
export class CoreModule {
  public static forRoot(): ModuleWithProviders<CoreModule> {
    return {
      ngModule: CoreModule,
      providers: [AuthenticationService, NotificationService, AuthorizationService]
    };
  }
}
