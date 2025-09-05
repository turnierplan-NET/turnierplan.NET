import { Component, OnInit, TemplateRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';
import { switchMap } from 'rxjs';

import { UserDto, UsersService } from '../../../api';
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
    NgClass
  ]
})
export class AdministrationPageComponent implements OnInit {
  protected loadingState: LoadingState = { isLoading: true };
  protected users: UserDto[] = [];
  protected currentUserId: string = '';

  protected userSelectedForDeletion?: UserDto;
  protected userSelectedForEditing?: UserDto;
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

  protected editButtonClicked(id: string, template: TemplateRef<unknown>): void {
    if (id === this.currentUserId) {
      return;
    }

    this.userSelectedForEditing = this.users.find((x) => x.id === id);

    if (this.userSelectedForEditing) {
      this.currentOffcanvas = this.offcanvasService.open(template, { position: 'end' });
    }
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
