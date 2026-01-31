import { Page } from '@playwright/test';
import { turnierplan } from '../consts/turnierplan';

export class OrganizationPage {
  constructor(private readonly page: Page) {}

  public async deleteOrganization(confirmText: string): Promise<void> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.settingsPageId)).click();
    await this.page.getByTestId(turnierplan.deleteWidget.confirmationField).fill(confirmText);
    await this.page.getByTestId(turnierplan.deleteWidget.deleteButton).click();
    await this.page.getByTestId(turnierplan.deleteWidget.confirmDeleteButton).click();
  }
}
