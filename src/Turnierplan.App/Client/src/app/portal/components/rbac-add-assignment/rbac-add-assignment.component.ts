import { Component, OnDestroy } from '@angular/core';
import { NgbActiveModal, NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { finalize, Observable, Subject } from 'rxjs';
import { NotificationService } from '../../../core/services/notification.service';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { FormsModule } from '@angular/forms';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';

type Step = 'SelectRole' | 'SelectPrincipal';

@Component({
  templateUrl: './rbac-add-assignment.component.html',
  styleUrl: './rbac-add-assignment.component.scss',
  imports: [TranslateDirective, NgClass, ActionButtonComponent, FormsModule, NgbTooltip, SmallSpinnerComponent, TranslatePipe]
})
export class RbacAddAssignmentComponent implements OnDestroy {
  protected readonly PrincipalKind = PrincipalKind;
  protected readonly availableRoles = Object.keys(Role) as Role[];

  protected step: Step = 'SelectRole';
  protected selectedRole?: Role = undefined;
  protected selectedPrincipalKind: PrincipalKind = PrincipalKind.User;
  protected searchPrincipalInput: string = '';
  protected isCreatingRoleAssignment = false;

  private readonly errorSubject$ = new Subject<unknown>();
  private targetScopeId: string = '';

  constructor(
    protected readonly modal: NgbActiveModal,
    private readonly notificationService: NotificationService
  ) {}

  public set scopeId(value: string) {
    this.targetScopeId = value;
  }

  public get error$(): Observable<unknown> {
    return this.errorSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.errorSubject$.complete();
  }

  protected previousStep(): void {
    if (this.step === 'SelectPrincipal') {
      this.step = 'SelectRole';
    }
  }

  protected nextStep(): void {
    if (this.step === 'SelectRole' && this.selectedRole !== undefined) {
      this.step = 'SelectPrincipal';
    }
  }

  protected selectRole(role: Role): void {
    this.selectedRole = role;
    this.step = 'SelectPrincipal';
  }

  protected addRoleAssignment(): void {
    if (this.isCreatingRoleAssignment || this.selectedRole === undefined || this.searchPrincipalInput.trim().length === 0) {
      return;
    }

    this.isCreatingRoleAssignment = true;

    const request: CreateRoleAssignmentEndpointRequest = {
      scopeId: this.targetScopeId,
      role: this.selectedRole,
      apiKeyId: this.selectedPrincipalKind === PrincipalKind.ApiKey ? this.searchPrincipalInput.trim() : null,
      userEmail: this.selectedPrincipalKind === PrincipalKind.User ? this.searchPrincipalInput.trim() : null
    };

    this.roleAssignmentsService
      .createRoleAssignment({ body: request })
      .pipe(finalize(() => (this.isCreatingRoleAssignment = false)))
      .subscribe({
        next: () => {
          this.notificationService.showNotification(
            'success',
            'Portal.RbacManagement.AddRoleAssignment.CreateSuccessToast.Title',
            'Portal.RbacManagement.AddRoleAssignment.CreateSuccessToast.Message'
          );
          this.modal.close();
        },
        error: (error: { status?: number }) => {
          if (error.status === 400) {
            this.notificationService.showNotification(
              'error',
              'Portal.RbacManagement.AddRoleAssignment.PrincipalNotFoundToast.Title',
              'Portal.RbacManagement.AddRoleAssignment.PrincipalNotFoundToast.Message'
            );
          } else if (error.status === 409) {
            this.notificationService.showNotification(
              'error',
              'Portal.RbacManagement.AddRoleAssignment.AssignmentAlreadyExists.Title',
              'Portal.RbacManagement.AddRoleAssignment.AssignmentAlreadyExists.Message'
            );
          } else {
            this.errorSubject$.next(error);
          }
        }
      });
  }
}
