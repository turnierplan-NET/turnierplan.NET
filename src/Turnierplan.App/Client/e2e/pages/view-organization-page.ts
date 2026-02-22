import { Page } from '@playwright/test';
import { turnierplan } from '../consts/turnierplan';

export class ViewOrganizationPage {
  constructor(private readonly page: Page) {}

  public async createTournament(name: string): Promise<void> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.tournamentsPageId)).click();
    await this.page.getByTestId(turnierplan.viewOrganizationPage.newTournamentButton).click();
    await this.page.getByTestId(turnierplan.createTournamentPage.tournamentNameField).fill(name);
    await this.page.getByTestId(turnierplan.createTournamentPage.confirmButton).click();
  }

  public async createApiKey(name: string): Promise<{ id: string; secret: string }> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.apiKeysPageId)).click();
    await this.page.getByTestId(turnierplan.viewOrganizationPage.newApiKeyButton).click();
    await this.page.getByTestId(turnierplan.createApiKeyPage.apiKeyNameField).fill(name);
    await this.page.getByTestId(turnierplan.createApiKeyPage.confirmButton).click();

    await this.page.getByTestId(turnierplan.createApiKeyPage.resultIdField).waitFor({ state: 'visible' });
    const resultId = await this.page.getByTestId(turnierplan.createApiKeyPage.resultIdField).inputValue();
    const resultSecret = await this.page.getByTestId(turnierplan.createApiKeyPage.resultSecretField).inputValue();

    await this.page.getByTestId(turnierplan.createApiKeyPage.doneButton).click();

    return { id: resultId, secret: resultSecret };
  }

  public async deleteApiKeyWithId(id: string, name: string): Promise<void> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.apiKeysPageId)).click();
    await this.page.getByTestId(turnierplan.viewOrganizationPage.deleteApiKeyButton(id)).click();
    await this.page.getByTestId(turnierplan.deleteWidget.confirmationField).fill(name);
    await this.page.getByTestId(turnierplan.deleteWidget.deleteButton).click();
    await this.page.getByTestId(turnierplan.deleteModal.confirmDeleteButton).click();
  }

  public async deleteOrganization(name: string): Promise<void> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.settingsPageId)).click();
    await this.page.getByTestId(turnierplan.deleteWidget.confirmationField).fill(name);
    await this.page.getByTestId(turnierplan.deleteWidget.deleteButton).click();
    await this.page.getByTestId(turnierplan.deleteModal.confirmDeleteButton).click();
  }

  public async openRoleAssignments(): Promise<void> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.settingsPageId)).click();
    await this.page.getByTestId(turnierplan.rbacWidget.openOffcanvasButton).click();
  }
}
