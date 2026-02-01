import { Component, OnDestroy } from '@angular/core';
import { finalize, Observable, Subject } from 'rxjs';
import { NotificationService } from '../../../core/services/notification.service';
import { NgbActiveOffcanvas, NgbModal, NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { RbacAddAssignmentComponent } from '../rbac-add-assignment/rbac-add-assignment.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { SmallSpinnerComponent } from '../../../core/components/small-spinner/small-spinner.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { RbacPrincipalComponent } from '../rbac-principal/rbac-principal.component';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';
import { TranslateDatePipe } from '../../pipes/translate-date.pipe';
import { Role } from '../../../api/models/role';
import { RoleAssignmentDto } from '../../../api/models/role-assignment-dto';
import { TurnierplanApi } from '../../../api/turnierplan-api';
import { deleteRoleAssignment } from '../../../api/fn/role-assignments/delete-role-assignment';
import { getRoleAssignments } from '../../../api/fn/role-assignments/get-role-assignments';

interface IRbacOffcanvasTarget {
  name: string;
  rbacScopeId: string;
}

@Component({
  templateUrl: './rbac-offcanvas.component.html',
  imports: [
    TranslateDirective,
    SmallSpinnerComponent,
    NgbTooltip,
    ActionButtonComponent,
    RbacPrincipalComponent,
    DeleteButtonComponent,
    TranslatePipe,
    TranslateDatePipe
  ]
})
export class RbacOffcanvasComponent implements OnDestroy {
  protected target!: IRbacOffcanvasTarget;
  protected targetIcon: string = '';
  protected isLoadingRoleAssignments = false;
  protected roleAssignments: { role: Role; assignments: RoleAssignmentDto[] }[] = [];
  protected roleAssignmentCount: number = 0;

  protected scopeTranslationKey: string = '';

  private readonly errorSubject$ = new Subject<unknown>();
  private targetIsOrganization: boolean = false;

  constructor(
    protected readonly offcanvas: NgbActiveOffcanvas,
    private readonly turnierplanApi: TurnierplanApi,
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
      case 'Image':
        this.targetIcon = 'image';
        break;
      case 'Organization':
        this.targetIcon = 'boxes';
        break;
      case 'PlanningRealm':
        this.targetIcon = 'ticket-perforated';
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

    this.loadRoleAssignments();
  }

  protected removeRoleAssignment(id: string): void {
    this.turnierplanApi.invoke(deleteRoleAssignment, { scopeId: this.target.rbacScopeId, roleAssignmentId: id }).subscribe({
      next: () => {
        this.roleAssignmentCount = 0;

        for (const entry of this.roleAssignments) {
          entry.assignments = entry.assignments.filter((x) => x.id !== id);
          this.roleAssignmentCount += entry.assignments.length;
        }

        this.roleAssignments = this.roleAssignments.filter((x) => x.assignments.length > 0);

        this.notificationService.showNotification(
          'success',
          'Portal.RbacManagement.DeletedSuccessToast.Title',
          'Portal.RbacManagement.DeletedSuccessToast.Message'
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

    if (
      assignment.role === Role.Owner &&
      this.targetIsOrganization &&
      this.roleAssignments.find((x) => x.role === Role.Owner)?.assignments.length === 1
    ) {
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
    const ref = this.modalService.open(RbacAddAssignmentComponent, {
      size: 'lg',
      fullscreen: 'lg',
      centered: true
    });

    const component = ref.componentInstance as RbacAddAssignmentComponent;

    component.scopeId = this.target.rbacScopeId;
    component.error$.subscribe((error) => this.errorSubject$.next(error));

    ref.closed.subscribe(() => this.loadRoleAssignments());
  }

  private loadRoleAssignments(): void {
    this.isLoadingRoleAssignments = true;

    this.turnierplanApi
      .invoke(getRoleAssignments, { scopeId: this.target.rbacScopeId })
      .pipe(finalize(() => (this.isLoadingRoleAssignments = false)))
      .subscribe({
        next: (roleAssignments) => {
          // This definition guarantees the correct ordering
          this.roleAssignments = Object.keys(Role).map((x) => ({ role: x as Role, assignments: [] as RoleAssignmentDto[] }));
          this.roleAssignmentCount = 0;

          for (const roleAssignment of roleAssignments) {
            this.roleAssignmentCount++;

            for (const entry of this.roleAssignments) {
              if (entry.role === roleAssignment.role) {
                entry.assignments.push(roleAssignment);
                break;
              }
            }
          }

          this.roleAssignments = this.roleAssignments.filter((x) => x.assignments.length > 0);
        },
        error: (error) => {
          this.errorSubject$.next(error);
        }
      });
  }
}
