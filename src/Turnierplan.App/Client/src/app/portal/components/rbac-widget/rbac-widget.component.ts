import { Component, EventEmitter, Input, OnInit, Output, TemplateRef } from '@angular/core';
import { NgbOffcanvas, NgbOffcanvasRef } from '@ng-bootstrap/ng-bootstrap';
import { PrincipalKind, Role, RoleAssignmentDto, RoleAssignmentsService } from '../../../api';
import { finalize } from 'rxjs';

export interface IRbacWidgetTarget {
  name: string;
  rbacScopeId: string;
}

@Component({
  standalone: false,
  selector: 'tp-rbac-widget',
  templateUrl: './rbac-widget.component.html'
})
export class RbacWidgetComponent {
  @Input()
  public translationKey: string = '';

  @Input()
  public target!: IRbacWidgetTarget;

  @Input()
  public targetIsOrganization: boolean = false;

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly PrincipalKind = PrincipalKind;

  protected isLoadingRoleAssignments = false;
  protected roleAssignments: { [key: string]: RoleAssignmentDto[] } = {};
  protected roleAssignmentCount: number = 0;
  protected currentOffcanvas?: NgbOffcanvasRef;

  constructor(
    private readonly offcanvasService: NgbOffcanvas,
    private readonly roleAssignmentsService: RoleAssignmentsService
  ) {}

  protected buttonClicked(template: TemplateRef<unknown>): void {
    this.loadRoleAssignments();
    this.currentOffcanvas = this.offcanvasService.open(template, { position: 'end' });
  }

  private loadRoleAssignments(): void {
    this.isLoadingRoleAssignments = true;

    this.roleAssignmentsService
      .getRoleAssignments({ scopeId: this.target.rbacScopeId })
      .pipe(finalize(() => (this.isLoadingRoleAssignments = false)))
      .subscribe({
        next: (roleAssignments) => {
          this.roleAssignments = {};
          this.roleAssignmentCount = 0;

          for (const roleAssignment of roleAssignments) {
            this.roleAssignmentCount++;

            if (roleAssignment.role in this.roleAssignments) {
              this.roleAssignments[roleAssignment.role].push(roleAssignment);
            } else {
              this.roleAssignments[roleAssignment.role] = [roleAssignment];
            }
          }
        },
        error: (error) => {
          this.errorOccured.emit(error);
        }
      });
  }

  protected removeRoleAssignment(id: string): void {
    // TODO: Implement this method
  }

  protected canDeleteAssignment(assignment: RoleAssignmentDto): boolean {
    if (assignment.isInherited) {
      return false;
    }

    if (assignment.role === Role.Owner && this.targetIsOrganization && this.roleAssignments[Role.Owner].length === 1) {
      // This is a special case which forbids deleting the only Owner assignment from an organization.
      return false;
    }

    return true;
  }
}
