const makeIdentifier = () => {
  return `e2e_${`${Math.random()}`.substring(2)}`
}

Cypress.Commands.add('login', () => {
  cy.visit('/portal/login');

  cy.get('input#email').type('admin@example.com');
  cy.get('input#password').type('P@ssw0rd');
  cy.get('button#loginButton').click();
})

Cypress.Commands.add('add_organization', () => {
  const organizationName = makeIdentifier();

  cy.get('#turnierplanLogoLink').click();
  cy.get('#newOrganizationButton').click();
  cy.get('input#create_organization_name').type(organizationName);
  cy.get('#createOrganizationConfirmButton').click()
  cy.get('span#pageFrameTitle').should('have.text', organizationName);

  return cy.wrap(organizationName);
})
