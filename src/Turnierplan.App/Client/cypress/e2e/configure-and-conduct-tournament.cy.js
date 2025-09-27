import { turnierplan } from '../support/turnierplan';
import { teamNames } from '../support/names';

it('create, configure and conduct a basic tournament', () => {
  cy.login();
  cy.add_organization().then(() => {
    // adding a tournament navigates to the configure page
    cy.add_tournament().then(() => {
      // add two groups
      cy.getx(turnierplan.configureTournamentPage.addGroupButton).click();
      cy.getx(turnierplan.configureTournamentPage.addGroupButton).click();

      // add four teams to each group
      const addTeam = (group, name) => {
        cy.getx(turnierplan.configureTournamentPage.addTeamButton(group)).click();
        cy.getx(turnierplan.addTeamDialog.teamNameField).type(name);
        cy.getx(turnierplan.addTeamDialog.confirmButton).click();
      };

      addTeam('A', teamNames[0]);
      addTeam('A', teamNames[1]);
      addTeam('A', teamNames[2]);
      addTeam('A', teamNames[3]);
      addTeam('B', teamNames[4]);
      addTeam('B', teamNames[5]);
      addTeam('B', teamNames[6]);
      addTeam('B', teamNames[7]);

      // enable finals round with semi-finals
      cy.getx(turnierplan.configureTournamentPage.enableFinalsRound).check();
      cy.getx(turnierplan.configureTournamentPage.firstFinalsRound).select('SemiFinal');

      // submit
      cy.getx(turnierplan.configureTournamentPage.submitButton).click();

      // check for success toast
      cy.contains('Turnier wurde aktualisiert');
      cy.contains('Die Änderungen am Turnier wurden erfolgreich durchgeführt und gespeichert');
    });
  });
});
