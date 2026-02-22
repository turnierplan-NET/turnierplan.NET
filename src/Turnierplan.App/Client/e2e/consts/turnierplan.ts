export const turnierplan = {
  createOrganizationPage: {
    confirmButton: 'create-organization-page-confirm-button',
    organizationNameField: 'create-organization-page-organization-name-field'
  },
  deleteWidget: {
    deleteButton: 'delete-widget-delete-button',
    confirmationField: 'delete-widget-confirmation-field'
  },
  deleteModal: {
    confirmDeleteButton: 'delete-modal-confirm-delete-button'
  },
  header: {
    logoLink: 'header-logo-link'
  },
  landingPage: {
    organizationsPageId: 0,
    newOrganizationButton: 'landing-page-new-organization-button'
  },
  loginPage: {
    userNameField: 'login-page-user-name-field',
    loginButton: 'login-page-login-button',
    passwordField: 'login-page-password-field'
  },
  pageFrame: {
    navigationTab: (id: number) => `page-frame-navigation-tab-${id}`,
    title: 'page-frame-title'
  },
  rbacWidget: {
    openOffcanvasButton: 'rbac-widget-open-offcanvas-button'
  },
  rbacOffcanvas: {
    doneButton: 'rbac-offcanvas-done-button',
    assignmentsCount: 'rbac-offcanvas-assignments-count'
  },
  viewOrganizationPage: {
    tournamentsPageId: 0,
    apiKeysPageId: 2,
    settingsPageId: 3,
    newTournamentButton: 'view-organization-page-new-tournament-button',
    newApiKeyButton: 'view-organization-page-new-api-key-button',
    deleteApiKeyButton: (id: string) => `view-organization-page-delete-api-key-button-${id}`
  },
  createTournamentPage: {
    confirmButton: 'create-tournament-page-confirm-button',
    tournamentNameField: 'create-tournament-page-tournament-name-field'
  },
  createApiKeyPage: {
    apiKeyNameField: 'create-api-key-page-api-key-name-field',
    confirmButton: 'create-api-key-page-confirm-button',
    resultIdField: 'create-api-key-page-result-id-field',
    resultSecretField: 'create-api-key-page-result-secret-field',
    doneButton: 'create-api-key-page-done-button'
  },
  configureTournamentPage: {
    addGroupButton: 'configure-tournament-page-add-group-button',
    shuffleGroupsButton: 'configure-tournament-page-shuffle-groups-button',
    addTeamButton: (alphabeticalId: string) => `configure-tournament-page-add-group-button-${alphabeticalId}`,
    enableFinalsRound: 'configure-tournament-page-enable-finals-round',
    firstFinalsRound: 'configure-tournament-page-first-finals-round',
    submitButton: 'configure-tournament-page-submit-button'
  },
  addTeamDialog: {
    teamNameField: 'add-team-dialog-team-name-field',
    confirmButton: 'add-team-dialog-confirm-button'
  },
  viewTournamentPage: {
    matchPlanPageId: 0,
    rankingsPageId: 3,
    matchPlan: {
      matchRow: (index: number) => `view-tournament-page-match-plan-match-row-${index}`
    },
    ranking: {
      teamName: (position: number) => `view-tournament-page-ranking-team-name-${position}`
    }
  },
  editMatchDialog: {
    scoreAField: 'edit-match-dialog-score-a-field',
    scoreBField: 'edit-match-dialog-score-b-field',
    saveButton: 'edit-match-dialog-save-button'
  },
  notification: (id: number) => `notification-${id}`
};
