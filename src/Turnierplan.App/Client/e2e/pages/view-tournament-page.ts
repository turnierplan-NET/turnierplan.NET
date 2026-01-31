import { Locator, Page } from '@playwright/test';
import { turnierplan } from '../consts/turnierplan';

export class ViewTournamentPage {
  constructor(private readonly page: Page) {}

  public async navigateTo(page: 'matchPlan' | 'ranking'): Promise<void> {
    const id = page === 'matchPlan' ? turnierplan.viewTournamentPage.matchPlanPageId : turnierplan.viewTournamentPage.rankingsPageId;
    await this.page.getByTestId(turnierplan.pageFrame.navigationTab(id)).click();
  }

  public async reportMatch(index: number, scoreA: number, scoreB: number): Promise<void> {
    await this.page.getByTestId(turnierplan.viewTournamentPage.matchPlan.matchRow(index)).click();
    await this.page.getByTestId(turnierplan.editMatchDialog.scoreAField).fill(`${scoreA}`);
    await this.page.waitForTimeout(100); // TODO: Fix test flakiness
    await this.page.getByTestId(turnierplan.editMatchDialog.scoreBField).fill(`${scoreB}`);
    await this.page.getByTestId(turnierplan.editMatchDialog.saveButton).click();
  }

  public getRankingTeamNameLocator(position: number): Locator {
    return this.page.getByTestId(turnierplan.viewTournamentPage.ranking.teamName(position));
  }
}
