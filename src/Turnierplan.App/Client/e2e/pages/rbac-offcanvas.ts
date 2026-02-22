import { Locator, Page } from '@playwright/test';
import { turnierplan } from '../consts/turnierplan';

export class RbacOffcanvas {
  constructor(private readonly page: Page) {}

  public getRoleAssignmentsCountLocator(): Locator {
    return this.page.getByTestId(turnierplan.rbacOffcanvas.assignmentsCount);
  }

  public async close(): Promise<void> {
    await this.page.getByTestId(turnierplan.rbacOffcanvas.doneButton).click();
  }
}
