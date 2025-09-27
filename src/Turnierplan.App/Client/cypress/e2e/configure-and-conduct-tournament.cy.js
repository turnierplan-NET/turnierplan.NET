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

      // report match outcomes
      const reportMatch = (index, scoreA, scoreB) => {
        cy.getx(turnierplan.viewTournamentPage.matchPlan.matchRow(index)).click();
        cy.getx(turnierplan.editMatchDialog.scoreAField).type(scoreA);
        cy.getx(turnierplan.editMatchDialog.scoreBField).type(scoreB);
        cy.getx(turnierplan.editMatchDialog.saveButton).click();
      };

      reportMatch(1, 0, 2);
      reportMatch(2, 2, 1);
      reportMatch(3, 1, 2);
      reportMatch(4, 3, 0);
      reportMatch(5, 2, 0);
      reportMatch(6, 0, 4);
      reportMatch(7, 2, 1);
      reportMatch(8, 1, 3);
      reportMatch(9, 0, 1);
      reportMatch(10, 2, 1);
      reportMatch(11, 1, 1);
      reportMatch(12, 2, 4);
      reportMatch(13, 3, 4);
      reportMatch(14, 4, 1);
      reportMatch(15, 0, 6);

      // switch to ranking page
      cy.getx(turnierplan.pageFrame.navigationTab(turnierplan.viewTournamentPage.rankingsPageId)).click();

      // validate the rankings
      const validateRanking = (position, expectedTeam) => {
        cy.getx(turnierplan.viewTournamentPage.ranking.teamName(position)).should('have.text', expectedTeam);
      };

      validateRanking(1, teamNames[6]);
      validateRanking(2, teamNames[4]);
      validateRanking(3, teamNames[0]);
      validateRanking(4, teamNames[1]);
      validateRanking(5, teamNames[7]);
      validateRanking(6, teamNames[3]);
      validateRanking(7, teamNames[2]);
      validateRanking(8, teamNames[5]);
    });
  });
});
