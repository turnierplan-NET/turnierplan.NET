import { turnierplan } from './turnierplan';

const makeIdentifier = () => {
  return `e2e_${`${Math.random()}`.substring(2)}`;
};

Cypress.Commands.add('getx', (id) => {
  return cy.get(`[data-cy="${id}"]`);
});

Cypress.Commands.add('login', () => {
  cy.visit('/portal/login');

  cy.getx(turnierplan.loginPage.emailField).type('admin@example.com');
  cy.getx(turnierplan.loginPage.passwordField).type('P@ssw0rd');
  cy.getx(turnierplan.loginPage.loginButton).click();
});

Cypress.Commands.add('add_organization', () => {
  const organizationName = makeIdentifier();

  cy.getx(turnierplan.header.logoLink).click();
  cy.getx(turnierplan.landingPage.newOrganizationButton).click();
  cy.getx(turnierplan.createOrganizationPage.organizationNameField).type(organizationName);
  cy.getx(turnierplan.createOrganizationPage.confirmButton).click();
  cy.getx(turnierplan.pageFrame.title).should('have.text', organizationName);

  return cy.wrap(organizationName);
});
