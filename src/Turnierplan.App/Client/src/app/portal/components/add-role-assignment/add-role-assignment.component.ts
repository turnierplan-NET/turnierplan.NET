import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

type Step = 'SelectRole' | 'SelectPrincipal' | 'Confirm';

@Component({
  standalone: false,
  templateUrl: './add-role-assignment.component.html',
  styleUrl: './add-role-assignment.component.scss'
})
export class AddRoleAssignmentComponent {
  protected step: Step = 'SelectRole';

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

  protected confirm(): void {}
}
