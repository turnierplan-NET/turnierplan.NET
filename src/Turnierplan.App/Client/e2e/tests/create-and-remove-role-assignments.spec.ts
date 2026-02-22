import { expect, test } from '@playwright/test';
import { createIdentifier } from '../utils/create-identifier';
import { LoginPage } from '../pages/login-page';
import { LandingPage } from '../pages/landing-page';
import { ViewOrganizationPage } from '../pages/view-organization-page';
import { RbacOffcanvas } from '../pages/rbac-offcanvas';

test('Create API key, then create and remove role assignment', async ({ page }) => {
  const organizationName = createIdentifier();

  await new LoginPage(page).login();
  await new LandingPage(page).createOrganization(organizationName);

  const organizationPage = new ViewOrganizationPage(page);
  const rbacOffcanvas = new RbacOffcanvas(page);

  await organizationPage.openRoleAssignments();
  await expect(rbacOffcanvas.getRoleAssignmentsCountLocator()).toHaveText('1 Zuweisung');
  await rbacOffcanvas.close();

  const apiKeyName = createIdentifier();
  await organizationPage.createApiKey(apiKeyName);

  await organizationPage.openRoleAssignments();
  await expect(rbacOffcanvas.getRoleAssignmentsCountLocator()).toHaveText('2 Zuweisungen');
  await rbacOffcanvas.close();

  // TODO: Delete API key

  await organizationPage.openRoleAssignments();
  await expect(rbacOffcanvas.getRoleAssignmentsCountLocator()).toHaveText('1 Zuweisung');
  await rbacOffcanvas.close();
});
