import { Component, OnDestroy } from '@angular/core';
import { PrincipalKind, Role } from '../../../api';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, Subject } from 'rxjs';

type Step = 'SelectRole' | 'SelectPrincipal';

@Component({
  standalone: false,
  templateUrl: './rbac-add-assignment.component.html',
  styleUrl: './rbac-add-assignment.component.scss'
})
export class RbacAddAssignmentComponent implements OnDestroy {
  protected readonly availableRoles = Object.keys(Role) as Role[];

  protected step: Step = 'SelectRole';
  protected selectedRole?: Role = undefined;
  protected selectedPrincipalKind: PrincipalKind = PrincipalKind.User;
  protected searchPrincipalInput: string = '';

  private readonly assignmentAddedSubject$ = new Subject<void>();

  private targetScopeId: string = '';

  constructor(protected readonly modal: NgbActiveModal) {}

  public set scopeId(value: string) {
    this.targetScopeId = value;
  }

  public get assignmentAdded$(): Observable<void> {
    return this.assignmentAddedSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.assignmentAddedSubject$.complete();
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
    if (this.selectedRole === undefined || this.searchPrincipalInput.trim().length === 0) {
      return;
    }

    // TODO: Add switch button to determine whether ApiKey/User
    //       Add notifications for various outcomes

    // TODO: Only if successful
    this.searchPrincipalInput = '';
    this.assignmentAddedSubject$.next();
  }

  protected readonly PrincipalKind = PrincipalKind;
}
