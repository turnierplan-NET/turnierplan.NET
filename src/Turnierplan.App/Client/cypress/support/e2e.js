import { turnierplan } from './turnierplan';
import { createIdentifier } from './names';

Cypress.Commands.add('getx', (id) => {
  if (typeof id !== 'string') {
    // assume id is an array of items
    id = id.join('-');
  }
  return cy.get(`[data-cy="${id}"]`);
});

Cypress.Commands.add('login', () => {
  cy.visit('/portal/login');

  cy.getx(turnierplan.loginPage.userNameField).type('admin');
  cy.getx(turnierplan.loginPage.passwordField).type('P@ssw0rd');
  cy.getx(turnierplan.loginPage.loginButton).click();
});

Cypress.Commands.add('add_organization', () => {
  const organizationName = createIdentifier();

  cy.getx(turnierplan.header.logoLink).click();
  cy.getx(turnierplan.landingPage.newOrganizationButton).click();
  cy.getx(turnierplan.createOrganizationPage.organizationNameField).type(organizationName);
  cy.getx(turnierplan.createOrganizationPage.confirmButton).click();
  cy.getx(turnierplan.pageFrame.title).should('have.text', organizationName);

  return cy.wrap(organizationName);
});

Cypress.Commands.add('add_tournament', () => {
  const tournamentName = createIdentifier();

  cy.getx(turnierplan.viewOrganizationPage.newTournamentButton).click();
  cy.getx(turnierplan.createTournamentPage.tournamentNameField).type(tournamentName);
  cy.getx(turnierplan.createTournamentPage.confirmButton).click();
});
