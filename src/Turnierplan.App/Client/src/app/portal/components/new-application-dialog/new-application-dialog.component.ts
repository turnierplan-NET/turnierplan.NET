import { Component, OnDestroy } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AbstractControl, FormArray, FormControl, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateDirective } from '@ngx-translate/core';
import { NgClass } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { AlertComponent } from '../alert/alert.component';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { PlanningRealmDto } from '../../../api/models/planning-realm-dto';
import { CreateApplicationEndpointRequest } from '../../../api/models/create-application-endpoint-request';
import { TournamentEditWarningComponent } from '../tournament-edit-warning/tournament-edit-warning.component';

const numberOfTeamsValidator = () => {
  return (formArray: AbstractControl): { [key: string]: any } | null => {
    if (!(formArray instanceof FormArray)) {
      return null;
    }

    const numberOfTeams = formArray.controls.reduce((sum, group) => {
      const numberOfTeamsControl = group.get('numberOfTeams');
      return sum + (numberOfTeamsControl ? numberOfTeamsControl.value : 0);
    }, 0);

    return numberOfTeams > 0 && numberOfTeams <= 30 ? null : { invalidNumberOfTeams: true };
  };
};

@Component({
  templateUrl: './new-application-dialog.component.html',
  imports: [
    TranslateDirective,
    FormsModule,
    ReactiveFormsModule,
    NgClass,
    AlertComponent,
    ActionButtonComponent,
    TournamentEditWarningComponent
  ]
})
export class NewApplicationDialogComponent implements OnDestroy {
  private static readonly ExplicitConfirmationThreshold = 5;

  protected readonly form = new FormGroup({
    contact: new FormControl('', { nonNullable: true, validators: Validators.required }),
    contactEmail: new FormControl('', { nonNullable: true, validators: [Validators.email] }),
    contactTelephone: new FormControl('', { nonNullable: true }),
    name: new FormControl('', { nonNullable: true, validators: Validators.required }),
    entries: new FormArray<FormGroup<{ tournamentClassId: FormControl<number>; numberOfTeams: FormControl<number> }>>([], {
      validators: [numberOfTeamsValidator()]
    })
  });

  protected readonly tournamentClassNames: { [key: number]: string } = {};

  protected explicitConfirmationTeamCount = 0;
  protected explicitConfirmationRequired = false;
  protected explicitConfirmationGiven = false;

  private readonly destroyed$ = new Subject<void>();

  constructor(protected readonly modal: NgbActiveModal) {}

  public ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

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
    const entriesFormArray = this.form.get('entries') as FormArray;

    for (const tournamentClass of planningRealm.tournamentClasses) {
      this.tournamentClassNames[tournamentClass.id] = tournamentClass.name;
      entriesFormArray.push(
        new FormGroup({
          tournamentClassId: new FormControl(tournamentClass.id),
          numberOfTeams: new FormControl(0, { validators: [Validators.min(0)] })
        })
      );
    }

    this.form.valueChanges.pipe(takeUntil(this.destroyed$)).subscribe({
      next: () => {
        const value = this.form.getRawValue();
        const totalNumberOfTeams = value.entries.reduce((sum, x) => sum + x.numberOfTeams, 0);

        if (this.explicitConfirmationRequired) {
          if (totalNumberOfTeams >= NewApplicationDialogComponent.ExplicitConfirmationThreshold) {
            this.explicitConfirmationTeamCount = totalNumberOfTeams;
          } else {
            this.explicitConfirmationRequired = false;
            this.explicitConfirmationGiven = false;
          }
        }
      }
    });
  }

  protected submit(): void {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();

    const totalNumberOfTeams = value.entries.reduce((sum, x) => sum + x.numberOfTeams, 0);

    if (totalNumberOfTeams >= NewApplicationDialogComponent.ExplicitConfirmationThreshold && !this.explicitConfirmationGiven) {
      this.explicitConfirmationTeamCount = totalNumberOfTeams;
      this.explicitConfirmationRequired = true;
      return;
    }

    const request: CreateApplicationEndpointRequest = {
      ...value,
      contactEmail: value.contactEmail === '' ? undefined : value.contactEmail,
      contactTelephone: value.contactTelephone === '' ? undefined : value.contactTelephone,
      entries: value.entries.filter((x) => x.numberOfTeams > 0)
    };

    this.modal.close(request);
  }
}
