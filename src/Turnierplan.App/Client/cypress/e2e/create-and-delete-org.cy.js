describe('User can create a new organization and delete it', () => {
  it('passes', () => {
    cy.login();
    cy.add_organization().then((organizationName) => {
      cy.contains('Einstellungen').click();
      cy.get('#deleteWidgetConfirmation').type(organizationName);
      cy.get('#deleteWidgetButton').click();
      cy.contains('Ihre Organisation wurde gelöscht'); // This is the confirmation toast
    });
  });
});
