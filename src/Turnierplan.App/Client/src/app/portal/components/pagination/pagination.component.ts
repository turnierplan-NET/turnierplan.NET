import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbPagination } from '@ng-bootstrap/ng-bootstrap';
import { TranslateDirective } from '@ngx-translate/core';

export interface PaginationParams {
  items: {
    length: number;
  };
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
}

@Component({
  selector: 'tp-pagination',
  imports: [NgbPagination, TranslateDirective],
  templateUrl: './pagination.component.html'
})
export class PaginationComponent {
  @Input()
  public pagination!: PaginationParams;

  @Output()
  public pageChange = new EventEmitter<number>();
}
