import { test, expect } from '@playwright/test';

test.beforeEach(async ({ page }) => {
  await page.goto('/');
});

test('hey', async ({ page }) => {
  await expect(page).toHaveTitle('turnierplan.NET');
});
