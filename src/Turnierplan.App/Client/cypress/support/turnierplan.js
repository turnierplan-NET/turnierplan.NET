export const turnierplan = {
  createOrganizationPage: {
    confirmButton: 'create-organization-page-confirm-button',
    organizationNameField: 'create-organization-page-organization-name-field'
  },
  deleteWidget: {
    confirmDeleteButton: 'delete-widget-confirm-delete-button',
    deleteButton: 'delete-widget-delete-button',
    confirmationField: 'delete-widget-confirmation-field'
  },
  header: {
    logoLink: 'header-logo-link'
  },
  landingPage: {
    newOrganizationButton: 'landing-page-new-organization-button'
  },
  loginPage: {
    userNameField: 'login-page-user-name-field',
    loginButton: 'login-page-login-button',
    passwordField: 'login-page-password-field'
  },
  pageFrame: {
    navigationTab: (id) => ['page-frame-navigation-tab', id],
    title: 'page-frame-title'
  },
  viewOrganizationPage: {
    settingsPageId: 3,
    newTournamentButton: 'view-organization-page-new-tournament-button'
  },
  createTournamentPage: {
    confirmButton: 'create-tournament-page-confirm-button',
    tournamentNameField: 'create-tournament-page-tournament-name-field'
  },
  configureTournamentPage: {
    addGroupButton: 'configure-tournament-page-add-group-button',
    shuffleGroupsButton: 'configure-tournament-page-shuffle-groups-button',
    addTeamButton: (alphabeticalId) => ['configure-tournament-page-add-group-button', alphabeticalId],
    enableFinalsRound: 'configure-tournament-page-enable-finals-round',
    firstFinalsRound: 'configure-tournament-page-first-finals-round',
    submitButton: 'configure-tournament-page-submit-button'
  },
  addTeamDialog: {
    teamNameField: 'add-team-dialog-team-name-field',
    confirmButton: 'add-team-dialog-confirm-button'
  }
};
