import { Component } from '@angular/core';
import { CreateApplicationEndpointRequest, PlanningRealmDto } from '../../../api';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AbstractControl, FormArray, FormControl, FormGroup, Validators } from '@angular/forms';

const atLeastOneTeamValidator = () => {
  return (formArray: AbstractControl): { [key: string]: any } | null => {
    if (!(formArray instanceof FormArray)) {
      return null;
    }

    const hasAtLeastOneTeam = formArray.controls.some((group) => {
      const numberOfTeamsControl = group.get('numberOfTeams');
      return numberOfTeamsControl && numberOfTeamsControl.value > 0;
    });

    return hasAtLeastOneTeam ? null : { atLeastOneTeamRequired: true };
  };
};

@Component({
  standalone: false,
  templateUrl: './new-application-dialog.component.html'
})
export class NewApplicationDialogComponent {
  protected readonly form = new FormGroup({
    contact: new FormControl('', { nonNullable: true, validators: Validators.required }),
    contactEmail: new FormControl('', { nonNullable: true, validators: [Validators.email] }),
    contactTelephone: new FormControl('', { nonNullable: true }),
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
    entries: new FormArray<FormGroup<{ tournamentClassId: FormControl<number>; numberOfTeams: FormControl<number> }>>([], {
      validators: [atLeastOneTeamValidator()]
    })
  });

  protected readonly tournamentClassNames: { [key: number]: string } = {};

  constructor(protected readonly modal: NgbActiveModal) {}

  protected get contact(): AbstractControl {
    return this.form.get('contact')!;
  }

  protected get contactEmail(): AbstractControl {
    return this.form.get('contactEmail')!;
  }

  protected get contactTelephone(): AbstractControl {
    return this.form.get('contactTelephone')!;
  }

  protected get name(): AbstractControl {
    return this.form.get('name')!;
  }

  protected get entries(): FormArray {
    return this.form.get('entries')! as FormArray;
  }

  public init(planningRealm: PlanningRealmDto): void {
    const classes = [...planningRealm.tournamentClasses].sort((a, b) => a.name.localeCompare(b.name));
    const entriesFormArray = this.form.get('entries') as FormArray;

    for (const tournamentClass of classes) {
      this.tournamentClassNames[tournamentClass.id] = tournamentClass.name;
      entriesFormArray.push(
        new FormGroup({
          tournamentClassId: new FormControl(tournamentClass.id),
          numberOfTeams: new FormControl(0, { validators: [Validators.min(0)] })
        })
      );
    }
  }

  protected submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();

    const request: CreateApplicationEndpointRequest = {
      ...value,
      contactEmail: value.contactEmail === '' ? undefined : value.contactEmail,
      contactTelephone: value.contactTelephone === '' ? undefined : value.contactTelephone,
      entries: value.entries.filter((x) => x.numberOfTeams > 0)
    };

    this.modal.close(request);
  }
}
