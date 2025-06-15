import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';

import { AuthenticationService } from '../../core/services/authentication.service';

export const isAdministratorGuard = (): CanActivateFn => {
  return () => {
    const router = inject(Router);
    const authenticationService = inject(AuthenticationService);

    return authenticationService
      .checkIfUserIsAdministrator()
      .pipe(map((isAdministrator) => (isAdministrator ? true : router.createUrlTree(['/portal']))));
  };
};
