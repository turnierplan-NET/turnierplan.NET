import { Injectable } from '@angular/core';

@Injectable()
export class LocalStorageService {
  public setNavigationTab(targetId: string, tabIndex: number): void {
    localStorage.setItem(`tp_navigationTab_${targetId}`, `${tabIndex}`);
  }

  public getNavigationTab(targetId: string): number | undefined {
    return this.getValueFromLocalStorage(`tp_navigationTab_${targetId}`, (x) => +x);
  }

  public setCurrentFolder(organizationId: string, folderId: string): void {
    localStorage.setItem(`tp_currentFolder_${organizationId}`, folderId);
  }

  public getCurrentFolderId(organizationId: string): string | undefined {
    return this.getValueFromLocalStorage(`tp_currentFolder_${organizationId}`, (x) => x);
  }

  public setDisplayAccumulateScore(enable: boolean): void {
    localStorage.setItem('tp_showAccumulatedScore', enable ? 'true' : 'false');
  }

  public isDisplayAccumulateScoreEnabled(): boolean {
    return this.getValueFromLocalStorage('tp_showAccumulatedScore', (x) => x === 'true') ?? false;
  }

  public setOpenTournamentInNewTab(enable: boolean): void {
    localStorage.setItem('tp_openTournamentInNewTab', enable ? 'true' : 'false');
  }

  public isOpenTournamentInNewTabEnabled(): boolean {
    return this.getValueFromLocalStorage('tp_openTournamentInNewTab', (x) => x === 'true') ?? true;
  }

  private getValueFromLocalStorage<T>(key: string, parser: (value: string) => T): T | undefined {
    const value = localStorage.getItem(key);
    return value !== null && value.length > 0 ? parser(value) : undefined;
  }
}
