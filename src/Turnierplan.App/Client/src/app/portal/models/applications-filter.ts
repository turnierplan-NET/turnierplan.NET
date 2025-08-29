export type InvitationLinkFilterValue = 'none' | number;

export interface ApplicationsFilter {
  searchTerm: string;
  tournamentClass: number[];
  invitationLink: InvitationLinkFilterValue[];
}

export const defaultApplicationsFilter: ApplicationsFilter = {
  searchTerm: '',
  tournamentClass: [],
  invitationLink: []
};
