import { expect, test } from '@playwright/test';
import { LoginPage } from '../pages/login-page';
import { LandingPage } from '../pages/landing-page';
import { createIdentifier } from '../utils/create-identifier';
import { OrganizationPage } from '../pages/organization-page';
import { turnierplan } from '../consts/turnierplan';
import { texts } from '../consts/texts';
import { getExpectedTitle } from '../utils/get-expected-title';

test('Create a new organization and delete it', async ({ page }) => {
  const organizationName = createIdentifier();

  await new LoginPage(page).login();
  await new LandingPage(page).createOrganization(organizationName);

  await expect(page).toHaveTitle(getExpectedTitle(organizationName));

  await new OrganizationPage(page).deleteOrganization(organizationName);

  await expect(page.getByTestId(turnierplan.notification(1))).toHaveText(texts.organizationDeletedNotification);
  await expect(page).toHaveURL(/\/portal$/);
});
