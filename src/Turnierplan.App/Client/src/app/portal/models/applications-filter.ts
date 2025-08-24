export type TournamentClassFilterValue = number;

export type InvitationLinkFilterValue = 'none' | number;

export interface ApplicationsFilter {
  searchTerm: string;
  tournamentClass: TournamentClassFilterValue[];
  invitationLink: InvitationLinkFilterValue[];
}

export const defaultApplicationsFilter: ApplicationsFilter = {
  searchTerm: '',
  tournamentClass: [],
  invitationLink: []
};
