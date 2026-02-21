import { Component, inject } from '@angular/core';
import { ActionButtonComponent } from '../action-button/action-button.component';
import { TranslateDirective, TranslatePipe } from '@ngx-translate/core';
import { E2eDirective } from '../../../core/directives/e2e.directive';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { map, Observable } from 'rxjs';
import { filter } from 'rxjs/operators';

export const showDeleteModal = (ngbModal: NgbModal, translationKey: string, targetObjectName: string): Observable<boolean> => {
  const ref = ngbModal.open(DeleteModalComponent, {
    size: 'md',
    fullscreen: 'md',
    centered: true
  });

  ref.componentInstance.translationKey = translationKey;
  ref.componentInstance.targetObjectName = targetObjectName;

  return ref.closed.pipe(
    map((x) => x as boolean),
    filter((x) => x)
  );
};

@Component({
  imports: [ActionButtonComponent, TranslatePipe, TranslateDirective, E2eDirective],
  templateUrl: './delete-modal.component.html'
})
export class DeleteModalComponent {
  public translationKey: string = '';
  public targetObjectName: string = '';

  protected readonly modal = inject(NgbActiveModal);
}
