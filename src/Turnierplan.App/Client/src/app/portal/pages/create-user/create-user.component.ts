import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { from, switchMap } from 'rxjs';

import { UsersService } from '../../../api';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';

@Component({
    templateUrl: './create-user.component.html',
    imports: [LoadingStateDirective, PageFrameComponent, FormsModule, ReactiveFormsModule, TranslateDirective, NgClass, ActionButtonComponent, TranslatePipe]
})
export class CreateUserComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: false };

  protected form = new FormGroup({
    userName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    eMail: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] })
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
    this.titleService.setTitleTranslated('Portal.CreateUser.Title');
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
