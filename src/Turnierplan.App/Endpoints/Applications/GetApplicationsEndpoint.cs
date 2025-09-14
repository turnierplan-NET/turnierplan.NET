using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Endpoints.Applications;

internal sealed class GetApplicationsEndpoint : EndpointBase<PaginationResultDto<ApplicationDto>>
{
    protected override HttpMethod Method => HttpMethod.Get;

    protected override string Route => "/api/planning-realms/{planningRealmId}/applications";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromRoute] PublicId planningRealmId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? searchTerm,
        [FromQuery] string[] tournamentClass,
        [FromQuery] string[] invitationLink,
        [FromQuery] bool? excludeLinkedTeams,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        if (!TournamentClassFilter.TryParseTournamentClassFilter(tournamentClass, out var tournamentClassFilter))
        {
            return Results.BadRequest("Invalid tournament class filter provided.");
        }

        if (!InvitationLinkFilter.TryParseInvitationLinkFilter(invitationLink, out var invitationLinkFilter))
        {
            return Results.BadRequest("Invalid invitation link filter provided.");
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.TournamentClasses | IPlanningRealmRepository.Includes.ApplicationsWithTeams);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ManageApplications))
        {
            return Results.Forbid();
        }

        var queryLogic = new QueryLogic(page, pageSize, searchTerm, tournamentClassFilter, invitationLinkFilter, excludeLinkedTeams);
        var result = queryLogic.Process(planningRealm, mapper);

        return Results.Ok(result);
    }

    private sealed class QueryLogic
    {
        private const int DefaultPageSize = 20;

        private readonly int _page;
        private readonly int _pageSize;
        private readonly string? _searchTerm;
        private readonly TournamentClassFilter? _tournamentClassFilter;
        private readonly InvitationLinkFilter? _invitationLinkFilter;
        private readonly bool _excludeLinkedTeams;

        public QueryLogic(int? page, int? pageSize, string? searchTerm, TournamentClassFilter? tournamentClassFilter, InvitationLinkFilter? invitationLinkFilter, bool? excludeLinkedTeams)
        {
            _page = page ?? 0;
            _pageSize = pageSize ?? DefaultPageSize;
            _searchTerm = searchTerm?.Trim();
            _tournamentClassFilter = tournamentClassFilter;
            _invitationLinkFilter = invitationLinkFilter;
            _excludeLinkedTeams = excludeLinkedTeams ?? false;
        }

        public PaginationResultDto<ApplicationDto> Process(PlanningRealm planningRealm, IMapper mapper)
        {
            var applications = planningRealm.Applications.AsEnumerable();

            if (_tournamentClassFilter.HasValue)
            {
                applications = applications.Where(application =>
                    application.Teams.Any(team => _tournamentClassFilter.Value.IncludeWithTournamentClassId.Contains(team.Class.Id)));
            }

            if (_invitationLinkFilter.HasValue)
            {
                applications = applications.Where(application => application.SourceLink is null
                    ? _invitationLinkFilter.Value.IncludeWithoutInvitationLink
                    : _invitationLinkFilter.Value.IncludeWithInvitationLinkId.Contains(application.SourceLink.Id));
            }

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                // If the search term is exactly 6 digits long, we expect that the user wants to search for a specific
                // tag. However, the user might also want to search for a specific telephone number or email address via
                // these 6 digits. This is considered in the search logic below.

                int? searchTag = _searchTerm.Length == 6 && _searchTerm.All(char.IsNumber)
                    ? int.Parse(_searchTerm)
                    : null;

                applications = applications.Where(x =>
                {
                    if (searchTag.HasValue && x.Tag == searchTag)
                    {
                        return true;
                    }

                    return x.Notes.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)
                        || x.Contact.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)
                        || x.ContactEmail?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true
                        || x.ContactTelephone?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true
                        || x.Comment?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true
                        || x.Teams.Any(team => team.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));
                });
            }

            if (_excludeLinkedTeams)
            {
                applications = applications.Where(application => application.Teams.Any(x => x.TeamLink is null));
            }

            applications = applications.OrderByDescending(x => x.CreatedAt);

            return PaginationHelper.Process<Application, ApplicationDto>(applications, _page, _pageSize, mapper);
        }
    }

    private record struct TournamentClassFilter(long[] IncludeWithTournamentClassId)
    {
        public static bool TryParseTournamentClassFilter(string[] queryParameters, out TournamentClassFilter? filter)
        {
            var sanitized = queryParameters
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();

            if (sanitized.Count == 0)
            {
                filter = null;
                return true;
            }

            var includeWithTournamentClassId = new List<long>();

            foreach (var value in sanitized)
            {
                if (long.TryParse(value, out var longValue))
                {
                    includeWithTournamentClassId.Add(longValue);
                }
                else
                {
                    filter = null;
                    return false;
                }
            }

            filter = new TournamentClassFilter(includeWithTournamentClassId.ToArray());
            return true;
        }
    }

    private record struct InvitationLinkFilter(bool IncludeWithoutInvitationLink, long[] IncludeWithInvitationLinkId)
    {
        public static bool TryParseInvitationLinkFilter(string[] queryParameters, out InvitationLinkFilter? filter)
        {
            var sanitized = queryParameters
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();

            if (sanitized.Count == 0)
            {
                filter = null;
                return true;
            }

            var includeWithoutInvitationLink = false;
            var includeWithInvitationLinkId = new List<long>();

            foreach (var value in sanitized)
            {
                if (value.Equals("none", StringComparison.Ordinal))
                {
                    includeWithoutInvitationLink = true;
                }
                else if (long.TryParse(value, out var longValue))
                {
                    includeWithInvitationLinkId.Add(longValue);
                }
                else
                {
                    filter = null;
                    return false;
                }
            }

            filter = new InvitationLinkFilter(includeWithoutInvitationLink, includeWithInvitationLinkId.ToArray());
            return true;
        }
    }
}
