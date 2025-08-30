import { turnierplan } from '../support/turnierplan';

it('create a new organization and delete it', () => {
  cy.login();
  cy.add_organization().then((organizationName) => {
    cy.getx(turnierplan.pageFrame.navigationTab(turnierplan.viewOrganizationPage.settingsPageId)).click();
    cy.getx(turnierplan.deleteWidget.confirmationField).type(organizationName);
    cy.getx(turnierplan.deleteWidget.confirmButton).click();
    cy.contains('Ihre Organisation wurde gelöscht'); // This is the confirmation toast
  });
});
