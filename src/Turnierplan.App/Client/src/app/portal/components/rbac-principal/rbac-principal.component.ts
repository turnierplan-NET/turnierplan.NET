import { Component, Input, OnInit } from '@angular/core';
import { PrincipalDto, PrincipalKind } from '../../../api';
import { PrincipalsService } from '../../../api/services/principals.service';
import { catchError, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { NgbTooltip } from '@ng-bootstrap/ng-bootstrap';
import { NgClass, AsyncPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
    selector: 'tp-rbac-principal',
    templateUrl: './rbac-principal.component.html',
    imports: [NgbTooltip, NgClass, AsyncPipe, TranslatePipe]
})
export class RbacPrincipalComponent implements OnInit {
  protected readonly PrincipalKind = PrincipalKind;

  protected displayName$?: Observable<{ found: boolean; value: string | undefined }>;

  @Input()
  public principal!: PrincipalDto;

  constructor(private readonly principalsService: PrincipalsService) {}

  public ngOnInit(): void {
    this.displayName$ = this.principalsService
      .getPrincipalName({
        principalKind: this.principal.kind,
        principalId: this.principal.principalId
      })
      .pipe(
        map((result) => ({ found: true, value: result.name })),
        catchError(() => of({ found: false, value: this.principal.principalId }))
      );
  }
}
