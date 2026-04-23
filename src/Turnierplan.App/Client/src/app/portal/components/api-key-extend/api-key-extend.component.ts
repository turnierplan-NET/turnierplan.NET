import { Component, OnDestroy } from '@angular/core';
import { ApiKeyDto } from '../../../api/models/api-key-dto';
import { Observable, Subject } from 'rxjs';

@Component({
  imports: [],
  templateUrl: './api-key-extend.component.html'
})
export class ApiKeyExtendComponent implements OnDestroy {
  private readonly errorSubject$ = new Subject<unknown>();

  public set apiKey(value: ApiKeyDto) {}

  public get error$(): Observable<unknown> {
    return this.errorSubject$.asObservable();
  }

  public ngOnDestroy(): void {
    this.errorSubject$.complete();
  }
}
