import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Role } from '../../../api';

type Step = 'SelectRole' | 'SelectPrincipal' | 'Confirm';

@Component({
  standalone: false,
  templateUrl: './add-role-assignment.component.html',
  styleUrl: './add-role-assignment.component.scss'
})
export class AddRoleAssignmentComponent {
  protected readonly availableRoles = Object.keys(Role) as Role[];

  protected step: Step = 'SelectRole';
  protected selectedRole: Role = Role.Reader;

  constructor(protected readonly modal: NgbActiveModal) {}

  protected previousStep(): void {
    switch (this.step) {
      case 'SelectRole':
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
      case 'Confirm':
        this.step = 'Confirm';
        break;
    }
  }

  protected selectRole(role: Role): void {
    this.selectedRole = role;
    this.nextStep();
  }

  protected confirm(): void {}
}
