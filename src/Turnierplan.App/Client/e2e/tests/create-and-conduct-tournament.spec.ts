import { expect, test } from '@playwright/test';
import { createIdentifier } from '../utils/create-identifier';
import { LoginPage } from '../pages/login-page';
import { LandingPage } from '../pages/landing-page';
import { ViewOrganizationPage } from '../pages/view-organization-page';
import { getExpectedTitle } from '../utils/get-expected-title';
import { ConfigureTournamentPage } from '../pages/configure-tournament-page';
import { teamNames } from '../utils/team-names';
import { turnierplan } from '../consts/turnierplan';
import { texts } from '../consts/texts';
import { ViewTournamentPage } from '../pages/view-tournament-page';

test('Create, configure and conduct a basic tournament', async ({ page }) => {
  await new LoginPage(page).login();
  await new LandingPage(page).createOrganization(createIdentifier());

  const tournamentName = createIdentifier();
  await new ViewOrganizationPage(page).createTournament(tournamentName);
  await expect(page).toHaveTitle(getExpectedTitle(tournamentName));

  const configureTournamentPage = new ConfigureTournamentPage(page);
  const viewTournamentPage = new ViewTournamentPage(page);

  // Add two groups
  await configureTournamentPage.addGroup();
  await configureTournamentPage.addGroup();

  // Add four teams to each group
  await configureTournamentPage.addTeam('A', teamNames[0]);
  await configureTournamentPage.addTeam('A', teamNames[1]);
  await configureTournamentPage.addTeam('A', teamNames[2]);
  await configureTournamentPage.addTeam('A', teamNames[3]);
  await configureTournamentPage.addTeam('B', teamNames[4]);
  await configureTournamentPage.addTeam('B', teamNames[5]);
  await configureTournamentPage.addTeam('B', teamNames[6]);
  await configureTournamentPage.addTeam('B', teamNames[7]);

  // Enable finals round with semi-finals
  await configureTournamentPage.enableFinalsRound('SemiFinal');

  // Submit and check for success toast
  await configureTournamentPage.submit();
  await expect(page.getByTestId(turnierplan.notification(1))).toHaveText(texts.configureTournamentNotification);

  // Report match outcomes
  await viewTournamentPage.navigateTo('matchPlan');
  await viewTournamentPage.reportMatch(1, 0, 2);
  await viewTournamentPage.reportMatch(2, 2, 1);
  await viewTournamentPage.reportMatch(3, 1, 2);
  await viewTournamentPage.reportMatch(4, 3, 0);
  await viewTournamentPage.reportMatch(5, 2, 0);
  await viewTournamentPage.reportMatch(6, 0, 4);
  await viewTournamentPage.reportMatch(7, 2, 1);
  await viewTournamentPage.reportMatch(8, 1, 3);
  await viewTournamentPage.reportMatch(9, 0, 1);
  await viewTournamentPage.reportMatch(10, 2, 1);
  await viewTournamentPage.reportMatch(11, 1, 1);
  await viewTournamentPage.reportMatch(12, 2, 4);
  await viewTournamentPage.reportMatch(13, 3, 4);
  await viewTournamentPage.reportMatch(14, 4, 1);
  await viewTournamentPage.reportMatch(15, 0, 6);

  // Validate the rankings
  await viewTournamentPage.navigateTo('ranking');
  await expect(viewTournamentPage.getRankingTeamNameLocator(1)).toHaveText(teamNames[6]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(2)).toHaveText(teamNames[4]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(3)).toHaveText(teamNames[0]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(4)).toHaveText(teamNames[1]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(5)).toHaveText(teamNames[7]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(6)).toHaveText(teamNames[3]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(7)).toHaveText(teamNames[2]);
  await expect(viewTournamentPage.getRankingTeamNameLocator(8)).toHaveText(teamNames[5]);
});
