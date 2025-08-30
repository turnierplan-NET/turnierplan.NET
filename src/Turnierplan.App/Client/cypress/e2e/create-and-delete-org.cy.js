import { ids } from '../support/ids';

describe('User can create a new organization and delete it', () => {
  it('passes', () => {
    cy.login();
    cy.add_organization().then((organizationName) => {
      cy.contains('Einstellungen').click();
      cy.getx(ids.deleteWidget.confirmationField).type(organizationName);
      cy.getx(ids.deleteWidget.confirmButton).click();
      cy.contains('Ihre Organisation wurde gelöscht'); // This is the confirmation toast
    });
  });
});
