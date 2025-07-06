import { Injectable } from '@angular/core';
import { Role } from '../../api';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable()
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
}
