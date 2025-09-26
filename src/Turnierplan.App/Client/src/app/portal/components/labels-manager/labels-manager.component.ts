import { Component, EventEmitter, Input, Output, TemplateRef } from '@angular/core';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { UpdatePlanningRealmFunc, ViewPlanningRealmComponent } from '../../pages/view-planning-realm/view-planning-realm.component';
import { ApplicationsFilter } from '../../models/applications-filter';
import { TranslateDirective } from '@ngx-translate/core';
import { Actions } from '../../../generated/actions';
import { AuthorizationService } from '../../../core/services/authorization.service';
import { AsyncPipe } from '@angular/common';
import { LabelComponent } from '../label/label.component';
import { RenameButtonComponent } from '../rename-button/rename-button.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { IsActionAllowedDirective } from '../../directives/is-action-allowed.directive';
import { NgbModal, NgbModalRef, NgbPopoverModule } from '@ng-bootstrap/ng-bootstrap';
import { DeleteButtonComponent } from '../delete-button/delete-button.component';

@Component({
  selector: 'tp-labels-manager',
  imports: [
    TranslateDirective,
    AsyncPipe,
    LabelComponent,
    RenameButtonComponent,
    ActionButtonComponent,
    IsActionAllowedDirective,
    NgbPopoverModule,
    DeleteButtonComponent
  ],
  templateUrl: './labels-manager.component.html'
})
export class LabelsManagerComponent {
  @Input()
  public planningRealm!: PlanningRealmDto;

  @Input()
  public updatePlanningRealm!: UpdatePlanningRealmFunc;

  @Output()
  public filterRequested = new EventEmitter<ApplicationsFilter>();

  protected readonly Actions = Actions;
  protected readonly availableColors = ViewPlanningRealmComponent.DefaultLabelColorCodes;

  protected confirmDeleteModal?: NgbModalRef;
  protected currentDeletingLabelId?: number;

  constructor(
    protected readonly authorizationService: AuthorizationService,
    private readonly modalService: NgbModal
  ) {}

  protected setLabelName(id: number, name: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const label = planningRealm.labels.find((x) => x.id == id);

      if (!label) {
        return false;
      }

      label.name = name;

      return true;
    });
  }

  protected setLabelDescription(id: number, description: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const label = planningRealm.labels.find((x) => x.id == id);

      if (!label) {
        return false;
      }

      label.description = description;

      return true;
    });
  }

  protected setLabelColor(id: any, color: string): void {
    this.updatePlanningRealm((planningRealm) => {
      const label = planningRealm.labels.find((x) => x.id == id);

      if (!label) {
        return false;
      }

      // The color comes with the '#' CSS prefix
      label.colorCode = color.substring(1);

      return true;
    });
  }

  protected deleteLabel(template: TemplateRef<unknown>, id: number): void {
    this.currentDeletingLabelId = id;
    this.confirmDeleteModal = this.modalService.open(template, {
      size: 'md',
      fullscreen: 'md',
      centered: true
    });
  }

  protected confirmDeleteClicked(): void {
    if (!this.currentDeletingLabelId) {
      return;
    }

    this.updatePlanningRealm((planningRealm) => {
      const index = planningRealm.labels.findIndex((x) => x.id === this.currentDeletingLabelId);

      if (index === -1) {
        return false;
      }

      planningRealm.labels.splice(index, 1);

      return true;
    });

    this.confirmDeleteModal?.close();
    this.currentDeletingLabelId = undefined;
  }

  protected searchApplicationsClicked(id: number): void {
    if (id < 0) {
      return;
    }

    this.filterRequested.emit({
      searchTerm: '',
      invitationLink: [],
      tournamentClass: [],
      label: [id]
    });
  }
}
