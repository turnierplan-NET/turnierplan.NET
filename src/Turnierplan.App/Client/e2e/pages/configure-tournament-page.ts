import { Page } from '@playwright/test';
import { turnierplan } from '../consts/turnierplan';

export class ConfigureTournamentPage {
  constructor(private readonly page: Page) {}

  public async addGroup(): Promise<void> {
    await this.page.getByTestId(turnierplan.configureTournamentPage.addGroupButton).click();
  }

  public async addTeam(groupAlphabeticalId: string, teamName: string): Promise<void> {
    await this.page.getByTestId(turnierplan.configureTournamentPage.addTeamButton(groupAlphabeticalId)).click();
    await this.page.getByTestId(turnierplan.addTeamDialog.teamNameField).fill(teamName);
    await this.page.getByTestId(turnierplan.addTeamDialog.confirmButton).click();
  }

  public async enableFinalsRound(firstFinalsRound: 'SemiFinal'): Promise<void> {
    await this.page.getByTestId(turnierplan.configureTournamentPage.enableFinalsRound).check();
    await this.page.getByTestId(turnierplan.configureTournamentPage.firstFinalsRound).selectOption(firstFinalsRound);
  }

  public async submit(): Promise<void> {
    await this.page.getByTestId(turnierplan.configureTournamentPage.submitButton).click();
  }
}
