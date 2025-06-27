import { Component, Input } from '@angular/core';
import { PrincipalDto, PrincipalKind } from '../../../api';

@Component({
  selector: 'tp-rbac-principal',
  standalone: false,
  templateUrl: './rbac-principal.component.html'
})
export class RbacPrincipalComponent {
  protected readonly PrincipalKind = PrincipalKind;

  @Input()
  public principal!: PrincipalDto;
}
