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

      cy.getx(turnierplan.configureTournamentPage.shuffleGroupsButton).click();
    });
  });
});
