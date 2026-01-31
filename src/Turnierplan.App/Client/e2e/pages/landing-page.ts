import { Page } from '@playwright/test';
import { turnierplan } from 'e2e/consts/turnierplan';

export class LandingPage {
  constructor(private readonly page: Page) {}

  public async createOrganization(name: string): Promise<void> {
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(turnierplan.landingPage.organizationsPageId)).click();
    await this.page.getByTestId(turnierplan.landingPage.newOrganizationButton).click();
    await this.page.getByTestId(turnierplan.createOrganizationPage.organizationNameField).fill(name);
    await this.page.getByTestId(turnierplan.createOrganizationPage.confirmButton).click();
  }
}
