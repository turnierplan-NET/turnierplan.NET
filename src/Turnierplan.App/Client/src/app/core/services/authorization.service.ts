import { Injectable } from '@angular/core';
import { Role } from '../../api';

@Injectable()
export class AuthorizationService {
  private readonly rolesCache: { [key: string]: Role[] } = {};

  constructor() {}

  public addRolesToCache(entityId: string, roles: Role[]): void {
    this.rolesCache[entityId] = roles;
    console.log(JSON.stringify(this.rolesCache));
  }
}
