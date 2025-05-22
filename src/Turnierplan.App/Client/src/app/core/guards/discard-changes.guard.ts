import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { map, take } from 'rxjs';

export interface DiscardChangesDetector {
  hasUnsavedChanges(): boolean;
}

export const discardChangesGuard: CanDeactivateFn<DiscardChangesDetector> = (component) => {
  const hasUnsavedChanges = component.hasUnsavedChanges();

  if (!hasUnsavedChanges) {
    return true;
  }

  const translateService = inject(TranslateService);

  return translateService.get('General.DiscardChanges').pipe(
    take(1),
    map((message) => window.confirm(message as string))
  );
};
