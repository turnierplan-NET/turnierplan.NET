import { Observable } from 'rxjs';
import { HttpResponse } from '@angular/common/http';

export const me: any = undefined;
export const logout: any = undefined;
export enum ChangePasswordFailedReason {
  InsecurePassword,
  InvalidCredentials,
  NewPasswordEqualsCurrent
}
export const setApiKeyExpiryDate: any = undefined;
export const getApiKeyUsage: any = undefined;
export const getApplicationChangeLog: any = undefined;
export enum ApplicationChangeLogType {
  TeamAdded,
  TeamRemoved,
  LabelAdded,
  LabelRemoved,
  TeamLinkCreated,
  TeamLinkDestroyed,
  NotesChanged,
  CommentChanged,
  ContactChanged,
  ContactEmailChanged,
  ContactTelephoneChanged,
  TeamRenamed
}
export enum ApplicationChangeLogProperty {
  PreviousValue,
  NewValue,
  TeamName,
  TournamentClassName,
  TournamentName,
  LabelName,
  LabelColorCode
}
export enum TeamComparisonMode {
  ByPoints,
  ByScoreDifference,
  ByScore,
  ByDirectComparison
}
export type ComputationConfigurationDto = any;
export type PublicId = any;
export type MatchPlanDocumentConfiguration = any;
export enum MatchPlanDateFormat {}
export enum MatchPlanOutcomes {
  HideOutcomeStructures
}
export type ReceiptsDocumentConfiguration = any;
export type DocumentDto = any;
export const getTournaments: any = undefined;
export const getDocuments: any = undefined;
export enum DocumentType {
  MatchPlan = 'MatchPlan',
  Receipts = 'Receipts'
}
export const getDocumentPdf: any = undefined;
export const setDocumentName: any = undefined;
export const getMatchPlanDocumentConfiguration: any = undefined;
export const getReceiptsDocumentConfiguration: any = undefined;
export const setMatchPlanDocumentConfiguration: any = undefined;
export const setReceiptsDocumentConfiguration: any = undefined;
export type SetMatchOutcomeEndpointRequest = any;
export enum MatchType {
  GroupMatch,
  Final,
  AdditionalPlayoff,
  ThirdPlacePlayoff
}
export enum MatchState {
  CurrentlyPlaying,
  NotStarted,
  Finished
}
export enum MatchOutcomeType {
  Standard = 'Standard',
  AfterOvertime = 'AfterOvertime',
  AfterPenalties = 'AfterPenalties',
  Null = 'Null',
  SpecialScoring = 'SpecialScoring'
}
export const exportApplications: any = undefined;
export type TournamentPlannerDto = any;
export type TournamentHeaderDto = any;
export const getImages: any = undefined;
export type ImageDto = any;
export const setImageName: any = undefined;
export const deleteImage: any = undefined;
export type InvitationLinkDto = any;
export type TournamentClassDto = any;
export type InvitationLinkEntryDto = any;
export type LabelDto = any;
export type PaginationResultDtoOfApplicationDto = any;
export const getApplications: any = undefined;
export type ApplicationTeamDto = any;
export type ApplicationDto = any;
export const setApplicationNotes: any = undefined;
export const setApplicationTeamName: any = undefined;
export const setApplicationTeamLabels: any = undefined;
export const deleteApplicationTeam: any = undefined;
export const createApplicationTeam: any = undefined;
export type StrictHttpResponse<T> = any;
export type SetApplicationNotesEndpointRequest = any;
export type SetApplicationContactEndpointRequest = any;
export const setApplicationContact: any = undefined;
export type SetApplicationContactEmailEndpointRequest = any;
export const setApplicationContactEmail: any = undefined;
export type SetApplicationContactTelephoneEndpointRequest = any;
export const setApplicationContactTelephone: any = undefined;
export type SetTournamentFolderEndpointRequest = any;
export type FolderDto = any;
export const getFolders: any = undefined;
export type CreateApplicationEndpointRequest = any;
export type CreateRankingOverwriteEndpointRequest = any;
export type RankingOverwriteDto = any;
export type PresentationConfigurationDto = any;
export enum ResultsMode {
  Default,
  OnlyMatchOutcomes,
  NoResults
}
export enum HeaderLineContent {
  CustomValue,
  TournamentName,
  OrganizerName
}
export const setTournamentPresentationConfiguration: any = undefined;
export const getTournamentPresentationConfiguration: any = undefined;
export enum RankingReason {
  ManuallyChanged
}
export enum PrincipalKind {
  User,
  ApiKey
}
export enum Role {
  Owner = 'Owner',
  Reader = 'Reader',
  Contributor = 'Contributor',
  Reporter = 'Reporter',
  ApplicationReader = 'ApplicationReader',
  ApplicationOfficer = 'ApplicationOfficer'
}
export type CreateRoleAssignmentEndpointRequest = any;
export const createRoleAssignment: any = undefined;
export type RoleAssignmentDto = any;
export const deleteRoleAssignment: any = undefined;
export const getRoleAssignments: any = undefined;
export type PrincipalDto = any;
export const getPrincipalName: any = undefined;
export type TournamentPlannerHeaderDto = any;
export const getTournamentPlanners: any = undefined;
export const getTournamentPlanner: any = undefined;
export type TeamLinkDto = any;
export type OrganizationDto = any;
export const setFolderName: any = undefined;
export type VenueDto = any;
export const getVenues: any = undefined;
export enum Visibility {
  Public = 'Public',
  Private = 'Private'
}
export type UserDto = any;
export const getUsers: any = undefined;
export type UpdateUserEndpointRequest = any;
export const updateUser: any = undefined;
export const deleteUser: any = undefined;
export type FinalsMatchDefinitionDto = any;
export type TournamentDto = any;
export const getTournament: any = undefined;
export const getAvailableFinalsMatchDefinitions: any = undefined;
export const configureTournament: any = undefined;
export type MatchPlanConfigurationDto = any;
export type ConfigureTournamentEndpointRequest = any;
export type FinalsPhaseConfigurationDto = any;
export type AdditionalPlayoffDto = any;
export type ConfigureTournamentEndpointRequestGroupEntry = any;
export type ConfigureTournamentEndpointRequestTeamEntry = any;
export type ApiKeyDto = any;
export const createApiKey: any = undefined;
export const createOrganization: any = undefined;
export const getOrganization: any = undefined;
export const createResourcePlanner: any = undefined;
export const createTournament: any = undefined;
export const createTournamentPlanner: any = undefined;
export const createUser: any = undefined;
export type CreateUserEndpointRequest = any;
export const createVenue: any = undefined;
export type SetTournamentMatchPlanEndpointRequestEntry = any;
export type GroupDto = any;
export type TeamDto = any;
export const getTournamentTeamSelectors: any = undefined;
export const setTournamentMatchPlan: any = undefined;
export type TeamSelectorDto = any;
export type FolderStatisticsDto = any;
export const getFolderStatistics: any = undefined;
export const getFolderTimetable: any = undefined;
export type FolderTimetableTournamentEntry = any;
export type FolderTimetableDto = any;
export const getOrganizations: any = undefined;
export const setOrganizationName: any = undefined;
export const deleteOrganization: any = undefined;
export const deleteApiKey: any = undefined;
export const setApiKeyStatus: any = undefined;
export const getApiKeys: any = undefined;
export type GetImagesEndpointResponse = any;
export type ResourcePlannerHeaderDto = any;
export const getResourcePlanners: any = undefined;
export type ResourcePlannerDto = any;
export const getResourcePlanner: any = undefined;
export type TournamentImagesDto = any;
export const getTournamentImages: any = undefined;
export const setMatchOutcome: any = undefined;
export const createDocument: any = undefined;
export const deleteDocument: any = undefined;
export const copyDocument: any = undefined;
export const deleteTournament: any = undefined;
export const setTeamName: any = undefined;
export const setTeamPriority: any = undefined;
export const setTeamEntryFeePaid: any = undefined;
export const setTeamOutOfCompetition: any = undefined;
export const setGroupName: any = undefined;
export const setTournamentComputationConfiguration: any = undefined;
export const setTournamentFolder: any = undefined;
export const setTournamentVenue: any = undefined;
export const setTournamentVisibility: any = undefined;
export enum SetTournamentImageEndpointRequestTarget {
  PrimaryLogo,
  SecondaryLogo,
  BannerImage
}
export const setTournamentImage: any = undefined;
export const setTournamentName: any = undefined;
export const createRankingOverwrite: any = undefined;
export const deleteRankingOverwrite: any = undefined;
export type UpdateTournamentPlannerEndpointRequest = any;
export const createApplication: any = undefined;
export const updateTournamentPlanner: any = undefined;
export const deleteTournamentPlanner: any = undefined;
export const getVenue: any = undefined;
export const updateVenue: any = undefined;
export const deleteVenue: any = undefined;
export const uploadImage$FormData: any = undefined;
export type MeEndpointResponse = any;
export type ApplicationChangeLogDto = any;
export type CreateApplicationTeamEndpointRequest = any;

export class TurnierplanApi {
  public invoke(p1: any, p2?: any, p3?: any): Observable<any> {
    return undefined!;
  }

  public invoke$Response(p1: any, p2?: any, p3?: any): Observable<HttpResponse<any>> {
    return undefined!;
  }
}
