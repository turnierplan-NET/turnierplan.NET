import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { from, switchMap } from 'rxjs';

import { UsersService } from '../../../api';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';

@Component({
  templateUrl: './create-user.component.html'
})
export class CreateUserComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: false };

  protected form = new FormGroup({
    userName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    eMail: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email, Validators.maxLength(100)] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(64)] })
  });

  constructor(
    private readonly userService: UsersService,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly titleService: TitleService
  ) {}

  protected get userNameControl(): AbstractControl {
    return this.form.get('userName')!;
  }

  protected get eMailControl(): AbstractControl {
    return this.form.get('eMail')!;
  }

  protected get passwordControl(): AbstractControl {
    return this.form.get('password')!;
  }

  public ngOnInit(): void {
    this.titleService.setTitleTranslated('Portal.CreateOrganization.Title');
  }

  protected confirmButtonClicked(): void {
    if (this.form.valid && !this.loadingState.isLoading) {
      this.loadingState = { isLoading: true };
      this.userService
        .createUser({ body: this.form.getRawValue() })
        .pipe(switchMap(() => from(this.router.navigate(['../..'], { relativeTo: this.route }))))
        .subscribe({
          next: () => {
            this.loadingState = { isLoading: false };
          },
          error: (error) => {
            this.loadingState = { isLoading: false, error: error };
          }
        });
    }
  }
}
