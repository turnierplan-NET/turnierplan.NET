import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map } from 'rxjs';

import { AuthenticationService } from '../../core/services/authentication.service';

export const hasRoleGuard = (roleId: string): CanActivateFn => {
  return () => {
    const router = inject(Router);
    const authenticationService = inject(AuthenticationService);

    return authenticationService.checkIfUserHasRole(roleId).pipe(map((hasRole) => (hasRole ? true : router.createUrlTree(['/portal']))));
  };
};
