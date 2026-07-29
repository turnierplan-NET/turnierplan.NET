import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ResourcePlannerDto } from '../../../api/models/resource-planner-dto';
import { ResourceAssignmentState } from '../../../api/models/resource-assignment-state';
import { ResourceType } from '../../../api/models/resource-type';
import { TranslateDirective } from '@ngx-translate/core';
import { ResourceGroupDto } from '../../../api/models/resource-group-dto';

type ResourcesViewModel = {
  columns: {
    groups: {
      id: number;
      name: string;
      notes?: string;
      start?: Date;
      end?: Date;
      resources: {
        id: number;
        type: ResourceType;
        name: string;
        notes?: string;
        state: ResourceAssignmentState;
      }[];
    }[];
  }[];
};

@Component({
  selector: 'tp-manage-resources',
  imports: [TranslateDirective],
  templateUrl: './manage-resources.component.html'
})
export class ManageResourcesComponent {
  @Input()
  public set resourcePlanner(value: ResourcePlannerDto) {
    this._resourcePlanner = value;
    this.updateViewModel();
  }

  @Output()
  public errorOccured = new EventEmitter<unknown>();

  protected readonly resourceType = ResourceType;

  protected viewModel?: ResourcesViewModel;
  private _resourcePlanner?: ResourcePlannerDto;

  private updateViewModel(): void {
    if (!this._resourcePlanner) {
      this.viewModel = undefined;
      return;
    }

    const dateColumns: { key: string | undefined; groups: ResourceGroupDto[] }[] = [];

    // Group the resource groups into columns based on the start/end date if available.
    // Any group without a start or end state will be placed in a separate column.

    for (const group of this._resourcePlanner.resourceGroups) {
      const rawDate = group.start ?? group.end;
      let columnKey: string | undefined = undefined;

      if (rawDate) {
        const jsDate = new Date(rawDate);
        columnKey = `${jsDate.getFullYear()}-${jsDate.getMonth() + 1}-${jsDate.getDate()}`; // Format must be sortable
      }

      const existingColumn = dateColumns.find((x) => x.key === columnKey);
      if (existingColumn) {
        existingColumn.groups.push(group);
      } else {
        dateColumns.push({
          key: columnKey,
          groups: [group]
        });
      }
    }

    const columnsForProcessing = [
      // First column: Groups without date information
      dateColumns.find((x) => x.key === undefined)?.groups ?? [],

      // Other columns: Groups with date information ordered ascending
      ...dateColumns
        .filter((x) => !!x.key)
        .sort((a, b) => a.key!.localeCompare(b.key!))
        .map((x) => x.groups)
    ];

    this.viewModel = {
      columns: columnsForProcessing
        .filter((x) => x.length > 0)
        .map((x) => ({
          groups: x.map((group) => ({
            id: group.id,
            name: group.name,
            notes: group.notes,
            start: group.start ? new Date(group.start) : undefined,
            end: group.end ? new Date(group.end) : undefined,
            resources: group.assignments.map((assignment) => {
              const resource = this._resourcePlanner?.resources.find((x) => x.id === assignment.resourceId)!;
              return {
                id: resource.id,
                type: resource.type,
                name: resource.name,
                notes: resource.notes,
                state: assignment.state
              };
            })
          }))
        }))
    };
  }
}
