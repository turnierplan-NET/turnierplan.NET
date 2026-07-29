import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ResourcePlannerDto } from '../../../api/models/resource-planner-dto';
import { ResourceAssignmentState } from '../../../api/models/resource-assignment-state';
import { ResourceType } from '../../../api/models/resource-type';
import { TranslateDirective } from '@ngx-translate/core';

type ResourcesViewModel = {
  columns: {
    groups: {
      id: number;
      name: string;
      description?: string;
      range?: {
        start: Date;
        end: Date;
      };
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
    // TODO: Convert resource planner to view model

    const time = (h: number, m: number) => {
      return new Date(2026, 7, 29, h, m, 0);
    };

    let id = 1;

    const column = (k: number) => ({
      groups: [
        {
          id: id++,
          name: 'Ansprechperson',
          range: {
            start: time(11, 30),
            end: time(17, 30)
          },
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        },
        {
          id: id++,
          name: 'Schicht 1',
          range: {
            start: time(10, 0),
            end: time(13, 0)
          },
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Requested
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        },
        {
          id: id++,
          name: 'Schicht 2',
          range: {
            start: time(13, 0),
            end: time(16, 0)
          },
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Requested
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Proposed
            }
          ]
        },
        {
          id: id++,
          name: 'Schicht 3',
          range: {
            start: time(16, 0),
            end: time(19, 0)
          },
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        },
        {
          id: id++,
          name: 'Kuchen',
          resources: [
            {
              id: id++,
              name: 'Quark Sahne Torte',
              type: ResourceType.Commodity,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Käsekuchen',
              type: ResourceType.Commodity,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Hasselnuss-Gugelhupf',
              type: ResourceType.Commodity,
              state: ResourceAssignmentState.Requested
            }
          ]
        },
        {
          id: id++,
          name: 'Turnierleitung',
          resources:
            k == 3
              ? []
              : [
                  {
                    id: id++,
                    name: 'Max Mustermann',
                    type: ResourceType.Personnel,
                    state: ResourceAssignmentState.Confirmed
                  }
                ]
        },
        {
          id: id++,
          name: 'Turnieraufsicht',
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        },
        {
          id: id++,
          name: 'Hallensprecher',
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            },
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        },
        {
          id: id++,
          name: 'Siegerehrung',
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        },
        {
          id: id++,
          name: 'Sanitäter',
          resources: [
            {
              id: id++,
              name: 'Max Mustermann',
              type: ResourceType.Personnel,
              state: ResourceAssignmentState.Confirmed
            }
          ]
        }
      ]
    });

    this.viewModel = {
      columns: [column(0), column(1), column(2), column(3), column(4)]
    };
  }
}
