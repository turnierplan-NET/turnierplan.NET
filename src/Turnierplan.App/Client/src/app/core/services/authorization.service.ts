import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Action } from '../../generated/actions';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthorizationService {
  private readonly rolesSubjects: { [key: string]: BehaviorSubject<Role[]> } = {};

  constructor() {}

  public addRolesToCache(entityId: string, roles: Role[]): void {
    if (entityId in this.rolesSubjects) {
      this.rolesSubjects[entityId].next(roles);
    } else {
      this.rolesSubjects[entityId] = new BehaviorSubject(roles);
    }
  }

  public getRoles$(entityId: string): Observable<Role[]> {
    if (!(entityId in this.rolesSubjects)) {
      this.rolesSubjects[entityId] = new BehaviorSubject([] as Role[]);
    }
    return this.rolesSubjects[entityId].asObservable();
  }

  public isActionAllowed$(entityId: string, action: Action): Observable<boolean> {
    return this.getRoles$(entityId).pipe(map((roles) => action.isAllowed(roles)));
  }

  public isActionNotAllowed$(entityId: string, action: Action): Observable<boolean> {
    return this.getRoles$(entityId).pipe(map((roles) => !action.isAllowed(roles)));
  }
}
