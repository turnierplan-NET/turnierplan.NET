import { Component, OnInit, TemplateRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';
import { switchMap } from 'rxjs';

import { AuthenticationService } from '../../../core/services/authentication.service';
import { NotificationService } from '../../../core/services/notification.service';
import { PageFrameNavigationTab, PageFrameComponent } from '../../components/page-frame/page-frame.component';
import { LoadingState, LoadingStateDirective } from '../../directives/loading-state.directive';
import { TitleService } from '../../services/title.service';
import { NavigationStart, Router, RouterLink } from '@angular/router';
import { filter } from 'rxjs/operators';
import { ActionButtonComponent } from '../../components/action-button/action-button.component';
import { BadgeComponent } from '../../components/badge/badge.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { DeleteWidgetComponent } from '../../components/delete-widget/delete-widget.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { NgClass } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AlertComponent } from '../../components/alert/alert.component';
import { UserDto } from '../../../api/models/user-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { getUsers } from '../../../api/fn/users/get-users';
import { UpdateUserEndpointRequest } from '../../../api/models/update-user-endpoint-request';
import { updateUser } from '../../../api/fn/users/update-user';
import { deleteUser } from '../../../api/fn/users/delete-user';

@Component({
  templateUrl: './administration-page.component.html',
  imports: [
    LoadingStateDirective,
    PageFrameComponent,
    ActionButtonComponent,
    RouterLink,
    BadgeComponent,
    TranslateDirective,
    DeleteWidgetComponent,
    TranslatePipe,
    TranslateDatePipe,
    NgClass,
    ReactiveFormsModule,
    AlertComponent
  ]
})
export class AdministrationPageComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: true };
  protected users: UserDto[] = [];
  protected currentUserId: string = '';

  protected userSelectedForDeletion?: UserDto;
  protected userSelectedForEditing?: UserDto;
  protected currentOffcanvas?: NgbOffcanvasRef;

  protected editUserForm = new FormGroup({
    userName: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    fullName: new FormControl('', { nonNullable: false }),
    eMail: new FormControl('', { nonNullable: false, validators: [Validators.email] }),
    isAdministrator: new FormControl(false, { nonNullable: true }),
    updatePassword: new FormControl(false, { nonNullable: true }),
    password: new FormControl('', { validators: [Validators.required] })
  });

  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.Administration.Pages.Users',
      icon: 'bi-people'
    }
  ];

  constructor(
    private readonly turnierplanApi: TurnierplanApi,
    private readonly titleService: TitleService,
    private readonly authenticationService: AuthenticationService,
    private readonly offcanvasService: NgbOffcanvas,
    private readonly notificationService: NotificationService,
    private readonly router: Router
  ) {
    this.authenticationService.authentication$.pipe(takeUntilDestroyed()).subscribe((userInfo) => (this.currentUserId = userInfo.id));

    this.router.events
      .pipe(
        takeUntilDestroyed(),
        filter((event) => event instanceof NavigationStart)
      )
      .subscribe({
        next: () => {
          this.currentOffcanvas?.close();
        }
      });
  }

  public ngOnInit(): void {
    this.titleService.setTitleTranslated('Portal.Administration.Title');

    this.turnierplanApi.invoke(getUsers).subscribe({
      next: (users) => {
        this.users = users;
        this.loadingState = { isLoading: false };
      },
      error: (error) => {
        this.loadingState = { isLoading: false, error: error };
      }
    });
  }

  protected editButtonClicked(id: string, template: TemplateRef<unknown>): void {
    if (id === this.currentUserId) {
      return;
    }

    this.userSelectedForEditing = this.users.find((x) => x.id === id);

    if (this.userSelectedForEditing) {
      this.editUserForm.setValue({
        userName: this.userSelectedForEditing.userName,
        fullName: this.userSelectedForEditing.fullName ?? '',
        eMail: this.userSelectedForEditing.eMail ?? '',
        isAdministrator: this.userSelectedForEditing.isAdministrator,
        updatePassword: false,
        password: ''
      });

      this.editUserForm.get('password')!.disable();
      this.editUserForm.markAsPristine({ onlySelf: false });

      this.currentOffcanvas = this.offcanvasService.open(template, { position: 'end' });
    }
  }

  protected updatePasswordToggled(): void {
    if (this.editUserForm.get('updatePassword')!.value) {
      this.editUserForm.get('password')?.enable();
    } else {
      this.editUserForm.get('password')?.disable();
    }
  }

  protected editConfirmed(userId: string): void {
    if (this.editUserForm.invalid) {
      return;
    }

    this.currentOffcanvas?.close();
    this.loadingState = { isLoading: true };

    const formValue = this.editUserForm.getRawValue();
    const request: UpdateUserEndpointRequest = {
      userName: formValue.userName,
      fullName: (formValue.fullName ?? '').length > 0 ? formValue.fullName : null,
      eMail: formValue.eMail,
      isAdministrator: formValue.isAdministrator,
      updatePassword: formValue.updatePassword,
      password: formValue.updatePassword ? formValue.password : undefined
    };

    this.turnierplanApi
      .invoke(updateUser, { id: userId, body: request })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getUsers)))
      .subscribe({
        next: (users) => {
          this.users = users;
          this.notificationService.showNotification(
            'success',
            'Portal.Administration.EditUser.SuccessToast.Title',
            'Portal.Administration.EditUser.SuccessToast.Message'
          );
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }

  protected deleteButtonClicked(id: string, template: TemplateRef<unknown>): void {
    if (id === this.currentUserId) {
      return;
    }

    this.userSelectedForDeletion = this.users.find((x) => x.id === id);

    if (this.userSelectedForDeletion) {
      this.currentOffcanvas = this.offcanvasService.open(template, { position: 'end' });
    }
  }

  protected deleteConfirmed(userId: string): void {
    this.currentOffcanvas?.close();
    this.loadingState = { isLoading: true };

    this.turnierplanApi
      .invoke(deleteUser, { id: userId })
      .pipe(switchMap(() => this.turnierplanApi.invoke(getUsers)))
      .subscribe({
        next: (users) => {
          this.users = users;
          this.notificationService.showNotification(
            'success',
            'Portal.Administration.DeleteUser.SuccessToast.Title',
            'Portal.Administration.DeleteUser.SuccessToast.Message'
          );
          this.loadingState = { isLoading: false };
        },
        error: (error) => {
          this.loadingState = { isLoading: false, error: error };
        }
      });
  }
}
