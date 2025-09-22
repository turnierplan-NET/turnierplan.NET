import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

import { AuthenticationService } from '../../../core/services/authentication.service';
import { TranslateDirective } from '@ngx-translate/core';
import { FormsModule } from '@angular/forms';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { NgClass } from '@angular/common';
import { E2eDirective } from '../../../core/directives/e2e.directive';

@Component({
  templateUrl: './login.component.html',
  imports: [TranslateDirective, FormsModule, SmallSpinnerComponent, NgClass, E2eDirective]
})
export class LoginComponent implements OnInit, OnDestroy {
  protected userName: string = '';
  protected password: string = '';

  protected isLoading = false;
  protected loginFailed = false;

  private redirectTarget = '/portal';
  private readonly destroyed$ = new Subject<void>();

  constructor(
    private readonly authenticationService: AuthenticationService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {}

  public ngOnInit(): void {
    if (this.authenticationService.isLoggedIn()) {
      void this.router.navigate(['portal']);
    } else {
      this.route.queryParamMap.pipe(takeUntil(this.destroyed$)).subscribe((params) => {
        this.redirectTarget = params.get('redirect_to') ?? '/portal';
        this.userName = params.get('user_name') ?? this.userName;
      });
    }
  }

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  protected attemptLogin(): void {
    if (this.isLoading || this.userName.trim().length === 0 || this.password.length === 0) {
      return;
    }

    this.isLoading = true;

    this.authenticationService.login(this.userName, this.password).subscribe((result) => {
      switch (result) {
        case 'success':
          void this.router.navigate([this.redirectTarget]);
          break;
        case 'failure':
          this.isLoading = false;
          this.loginFailed = true;
      }
    });
  }
}
