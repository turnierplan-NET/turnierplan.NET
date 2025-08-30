// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************

import { ids } from './ids';

const makeIdentifier = () => {
  return `e2e_${`${Math.random()}`.substring(2)}`;
};

Cypress.Commands.add('getx', (id) => {
  return cy.get(`[data-cy="${id}"]`);
});

Cypress.Commands.add('login', () => {
  cy.visit('/portal/login');

  cy.getx(ids.loginPage.emailField).type('admin@example.com');
  cy.getx(ids.loginPage.passwordField).type('P@ssw0rd');
  cy.getx(ids.loginPage.loginButton).click();
});

Cypress.Commands.add('add_organization', () => {
  const organizationName = makeIdentifier();

  cy.getx(ids.header.logoLink).click();
  cy.getx(ids.landingPage.newOrganizationButton).click();
  cy.getx(ids.createOrganizationPage.organizationNameField).type(organizationName);
  cy.getx(ids.createOrganizationPage.confirmButton).click();
  cy.getx(ids.pageFrame.title).should('have.text', organizationName);

  return cy.wrap(organizationName);
});
