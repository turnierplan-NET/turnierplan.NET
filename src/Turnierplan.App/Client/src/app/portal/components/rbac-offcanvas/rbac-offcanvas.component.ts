import { Component, OnDestroy } from '@angular/core';
import { finalize, Observable, Subject } from 'rxjs';
import { PrincipalKind, Role, RoleAssignmentDto, RoleAssignmentsService } from '../../../api';
import { NotificationService } from '../../../core/services/notification.service';
import { DocumentCopyComponent } from '../document-copy/document-copy.component';
import { AddRoleAssignmentComponent } from '../add-role-assignment/add-role-assignment.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

interface IRbacOffcanvasTarget {
  name: string;
  rbacScopeId: string;
}

@Component({
  standalone: false,
  templateUrl: './rbac-offcanvas.component.html'
})
export class RbacOffcanvasComponent implements OnDestroy {
  protected readonly PrincipalKind = PrincipalKind;

  protected target!: IRbacOffcanvasTarget;
  protected targetIcon: string = '';
  protected isLoadingRoleAssignments = false;
  protected roleAssignments: { [key: string]: RoleAssignmentDto[] } = {};
  protected roleAssignmentCount: number = 0;

  protected scopeTranslationKey: string = '';

  private readonly errorSubject$ = new Subject<unknown>();
  private targetIsOrganization: boolean = false;

  constructor(
    private readonly roleAssignmentsService: RoleAssignmentsService,
    private readonly notificationService: NotificationService,
    private readonly modalService: NgbModal
  ) {}

  public get error$(): Observable<unknown> {
    return this.errorSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.errorSubject$.complete();
  }

  public setTarget(target: IRbacOffcanvasTarget) {
    this.target = target;

    const scopeType = target.rbacScopeId.substring(0, target.rbacScopeId.indexOf(':'));
    this.targetIsOrganization = scopeType === 'Organization';
    this.scopeTranslationKey = `Portal.RbacManagement.ScopeType.${scopeType}`;

    switch (scopeType) {
      case 'Folder':
        this.targetIcon = 'folder2-open';
        break;
      case 'Organization':
        this.targetIcon = 'boxes';
        break;
      case 'Tournament':
        this.targetIcon = 'trophy';
        break;
      case 'Venue':
        this.targetIcon = 'buildings';
        break;
      default:
        this.targetIcon = 'question-lg';
        break;
    }

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
          this.errorSubject$.next(error);
        }
      });
  }

  protected removeRoleAssignment(id: string): void {
    this.roleAssignmentsService.deleteRoleAssignment({ scopeId: this.target.rbacScopeId, roleAssignmentId: id }).subscribe({
      next: () => {
        this.roleAssignmentCount = 0;

        for (const key of Object.keys(this.roleAssignments)) {
          const filtered = this.roleAssignments[key].filter((x) => x.id !== id);
          this.roleAssignmentCount += filtered.length;

          if (filtered.length > 0) {
            this.roleAssignments[key] = filtered;
          } else {
            delete this.roleAssignments[key];
          }
        }

        this.notificationService.showNotification(
          'success',
          'Portal.RbacManagement.SuccessToast.Title',
          'Portal.RbacManagement.SuccessToast.Message'
        );
      },
      error: (error) => {
        this.errorSubject$.next(error);
      }
    });
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

  protected navigateToScope(scopeId: string, scopeName: string): void {
    this.setTarget({
      rbacScopeId: scopeId,
      name: scopeName
    });
  }

  protected showAddRoleAssignmentDialog(): void {
    const ref = this.modalService.open(AddRoleAssignmentComponent, {
      size: 'lg',
      fullscreen: 'lg',
      centered: true
    });
  }
}
