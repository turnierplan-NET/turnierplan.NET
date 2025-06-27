import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PrincipalDto, PrincipalKind, Role } from '../../../api';

type Step = 'SelectRole' | 'SelectPrincipal' | 'Confirm';

@Component({
  standalone: false,
  templateUrl: './add-role-assignment.component.html',
  styleUrl: './add-role-assignment.component.scss'
})
export class AddRoleAssignmentComponent {
  protected readonly availableRoles = Object.keys(Role) as Role[];

  protected step: Step = 'SelectRole';
  protected selectedRole?: Role = undefined;
  protected selectedPrincipals: PrincipalDto[] = [];
  protected searchPrincipalInput: string = '';

  private targetScopeId: string = '';

  constructor(protected readonly modal: NgbActiveModal) {}

  public set scopeId(value: string) {
    this.targetScopeId = value;
  }

  protected previousStep(): void {
    switch (this.step) {
      case 'SelectPrincipal':
        this.step = 'SelectRole';
        break;
      case 'Confirm':
        this.step = 'SelectPrincipal';
        break;
    }
  }

  protected nextStep(): void {
    switch (this.step) {
      case 'SelectRole':
        this.step = 'SelectPrincipal';
        break;
      case 'SelectPrincipal':
        if (this.selectedPrincipals.length > 0) {
          this.step = 'Confirm';
        }
        break;
    }
  }

  protected selectRole(role: Role): void {
    this.selectedRole = role;
    this.nextStep();
  }

  protected searchPrincipal(): void {
    // TODO: Send request and, if not already present, add to list
    //       Add notifications for various outcomes

    this.selectedPrincipals.push({
      principalId: 'blubb',
      kind: PrincipalKind.User
    });

    // TODO Only reset if successful
    this.searchPrincipalInput = '';
  }

  protected removePrincipal(index: number): void {
    // Can't use .slice(index, 1) because that wouldn't trigger Angular change detection
    this.selectedPrincipals = this.selectedPrincipals.filter((_, i) => i !== index);
  }

  protected confirm(): void {
    if (!this.selectedRole || this.selectedPrincipals.length === 0) {
      return;
    }

    this.modal.close();
  }
}
