import { Page } from '@playwright/test';
import { turnierplan } from 'e2e/consts/turnierplan';

export class LoginPage {
  constructor(private readonly page: Page) {}

  public async login(): Promise<void> {
    await this.page.goto('/portal/login');

    await this.page.getByTestId(turnierplan.loginPage.userNameField).fill('admin');
    await this.page.getByTestId(turnierplan.loginPage.passwordField).fill('P@ssw0rd');
    await this.page.getByTestId(turnierplan.loginPage.loginButton).click();

    await this.page.waitForURL('/portal');
  }
}
