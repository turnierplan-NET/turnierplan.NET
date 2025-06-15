import { Component, OnInit, TemplateRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';
import { switchMap } from 'rxjs';

import { UserDto, UsersService } from '../../../api';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { NotificationService } from '../../../core/services/notification.service';
import { PageFrameNavigationTab } from '../../components/page-frame/page-frame.component';
import { LoadingState } from '../../directives/loading-state/loading-state.directive';
import { TitleService } from '../../services/title.service';

@Component({
  standalone: false,
  templateUrl: './administration-page.component.html'
})
export class AdministrationPageComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: true };
  protected users: UserDto[] = [];
  protected currentUserId: string = '';

  protected userSelectedForDeletion?: UserDto;
  protected currentOffcanvas?: NgbOffcanvasRef;

  protected pages: PageFrameNavigationTab[] = [
    {
      id: 0,
      title: 'Portal.Administration.Pages.Users',
      icon: 'bi-people'
    }
  ];

  constructor(
    private readonly userService: UsersService,
    private readonly titleService: TitleService,
    private readonly authenticationService: AuthenticationService,
    private readonly offcanvasService: NgbOffcanvas,
    private readonly notificationService: NotificationService
  ) {
    this.authenticationService.authentication$.pipe(takeUntilDestroyed()).subscribe((userInfo) => (this.currentUserId = userInfo.id));
  }

  public ngOnInit(): void {
    this.titleService.setTitleTranslated('Portal.Administration.Title');

    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
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

    this.userService
      .deleteUser({ id: userId })
      .pipe(switchMap(() => this.userService.getUsers()))
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
