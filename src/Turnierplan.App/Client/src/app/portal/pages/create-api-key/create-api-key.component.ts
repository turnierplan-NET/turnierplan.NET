import { Component } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, switchMap } from 'rxjs';

import { ApiKeyDto, OrganizationDto, ApiKeysService, OrganizationsService } from '../../../api';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';

@Component({
  templateUrl: './create-api-key.component.html'
})
export class CreateApiKeyComponent {
  protected loadingState: LoadingState = { isLoading: false };
  protected organization?: OrganizationDto;
  protected createdApiKey?: ApiKeyDto;

  /* eslint-disable @typescript-eslint/unbound-method */
  protected form = new FormGroup({
    name: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(25)] }),
    description: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(250)] }),
    validity: new FormControl(30, { nonNullable: true, validators: [Validators.min(1), Validators.max(180)] })
  });
  /* eslint-enable @typescript-eslint/unbound-method */

  constructor(
    private readonly organizationService: OrganizationsService,
    private readonly apiKeyService: ApiKeysService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly titleService: TitleService
  ) {
    this.route.paramMap
      .pipe(
        takeUntilDestroyed(),
        switchMap((params) => {
          const organizationId = params.get('id');
          if (organizationId === null) {
            this.loadingState = { isLoading: false };
            return of(undefined);
          }
          this.loadingState = { isLoading: true };
          return this.organizationService.getOrganization({ id: organizationId });
        })
      )
      .subscribe({
        next: (result) => {
          this.organization = result;
          this.titleService.setTitleFrom(result);
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected get nameControl(): AbstractControl {
    return this.form.get('name')!;
  }

  protected get descriptionControl(): AbstractControl {
    return this.form.get('description')!;
  }

  protected confirmButtonClicked(): void {
    if (this.form.valid && !this.loadingState.isLoading && this.organization) {
      this.loadingState = { isLoading: true };
      this.apiKeyService
        .createApiKey({
          body: {
            organizationId: this.organization.id,
            ...this.form.getRawValue()
          }
        })
        .subscribe({
          next: (result) => {
            this.loadingState = { isLoading: false };
            this.createdApiKey = result;
          },
          error: (error) => {
            this.loadingState = { isLoading: false, error: error };
          }
        });
    }
  }
}
