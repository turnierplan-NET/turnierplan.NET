export type InvitationLinkFilterValue = 'none' | number;

export interface ApplicationsFilter {
  searchTerm: string;
  tournamentClass: number[];
  invitationLink: InvitationLinkFilterValue[];
  label: number[];
}

export const defaultApplicationsFilter: ApplicationsFilter = {
  searchTerm: '',
  tournamentClass: [],
  invitationLink: [],
  label: []
};

export const applicationsFilterToQueryParameters = (filter: ApplicationsFilter) => ({
  searchTerm: filter.searchTerm.trim() === '' ? undefined : filter.searchTerm,
  tournamentClass: filter.tournamentClass.map((x) => `${x}`),
  invitationLink: filter.invitationLink.map((x) => `${x}`),
  label: filter.label.map((x) => `${x}`)
});
