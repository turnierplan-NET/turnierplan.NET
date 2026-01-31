import { turnierplan } from './turnierplan';
import { createIdentifier } from './names';

Cypress.Commands.add('add_tournament', () => {
  const tournamentName = createIdentifier();

  cy.getx(turnierplan.viewOrganizationPage.newTournamentButton).click();
  cy.getx(turnierplan.createTournamentPage.tournamentNameField).type(tournamentName);
  cy.getx(turnierplan.createTournamentPage.confirmButton).click();
});
