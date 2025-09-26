using System.Globalization;
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
        [FromQuery] string[] label,
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IMapper mapper)
    {
        if (!IdBasedFilterWithoutNone.TryParse(tournamentClass, out var tournamentClassFilter))
        {
            return Results.BadRequest("Invalid tournament class filter provided.");
        }

        if (!IdBasedFilterWithNone.TryParse(invitationLink, out var invitationLinkFilter))
        {
            return Results.BadRequest("Invalid invitation link filter provided.");
        }

        if (!IdBasedFilterWithoutNone.TryParse(label, out var labelFilter))
        {
            return Results.BadRequest("Invalid label filter provided.");
        }

        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Includes.TournamentClasses | IPlanningRealmRepository.Includes.ApplicationsWithTeamsAndTournamentLinks);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ManageApplications))
        {
            return Results.Forbid();
        }

        var queryLogic = new QueryLogic(page, pageSize, searchTerm, tournamentClassFilter, invitationLinkFilter, labelFilter);
        var result = queryLogic.Process(planningRealm, mapper);

        return Results.Ok(result);
    }

    private sealed class QueryLogic
    {
        private const int DefaultPageSize = 20;

        private readonly int _page;
        private readonly int _pageSize;
        private readonly string? _searchTerm;
        private readonly IdBasedFilterWithoutNone? _tournamentClassFilter;
        private readonly IdBasedFilterWithNone? _invitationLinkFilter;
        private readonly IdBasedFilterWithoutNone? _labelFilter;

        public QueryLogic(
            int? page,
            int? pageSize,
            string? searchTerm,
            IdBasedFilterWithoutNone? tournamentClassFilter,
            IdBasedFilterWithNone? invitationLinkFilter,
            IdBasedFilterWithoutNone? labelFilter)
        {
            _page = page ?? 0;
            _pageSize = pageSize ?? DefaultPageSize;
            _searchTerm = searchTerm?.Trim();
            _tournamentClassFilter = tournamentClassFilter;
            _invitationLinkFilter = invitationLinkFilter;
            _labelFilter = labelFilter;
        }

        public PaginationResultDto<ApplicationDto> Process(PlanningRealm planningRealm, IMapper mapper)
        {
            var applications = planningRealm.Applications.AsEnumerable();

            if (_tournamentClassFilter is not null)
            {
                applications = applications.Where(application =>
                    application.Teams.Any(team => _tournamentClassFilter.Value.IncludeWithId.Contains(team.Class.Id)));
            }

            if (_invitationLinkFilter is not null)
            {
                applications = applications.Where(application => application.SourceLink is null
                    ? _invitationLinkFilter.Value.IncludeWithNone
                    : _invitationLinkFilter.Value.IncludeWithId.Contains(application.SourceLink.Id));
            }

            if (_labelFilter is not null)
            {
                applications = applications.Where(application =>
                    application.Teams.Any(team => team.Labels.Any(label => _labelFilter.Value.IncludeWithId.Contains(label.Id))));
            }

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                // If the search term is exactly 6 digits long, we expect that the user wants to search for a specific
                // tag. However, the user might also want to search for a specific telephone number or email address via
                // these 6 digits. This is considered in the search logic below.
                int? searchTag = _searchTerm.Length == 6 && _searchTerm.All(char.IsNumber)
                    ? int.Parse(_searchTerm, CultureInfo.InvariantCulture)
                    : null;

                // If the search term starts with an exclamation mark, the rest is interpreted as an exact id match on an application team.
                long? searchApplicationTeamId = _searchTerm.Length >= 2 && _searchTerm[0] == '!' && _searchTerm.Skip(1).All(char.IsNumber)
                    ? int.Parse(_searchTerm[1..], CultureInfo.InvariantCulture)
                    : null;

                applications = applications.Where(x =>
                {
                    if (searchTag.HasValue && x.Tag == searchTag)
                    {
                        return true;
                    }

                    if (searchApplicationTeamId.HasValue)
                    {
                        return x.Teams.Any(team => team.Id == searchApplicationTeamId);
                    }

                    return x.Notes.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)
                        || x.Contact.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase)
                        || x.ContactEmail?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true
                        || x.ContactTelephone?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true
                        || x.Comment?.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) == true
                        || x.Teams.Any(team => team.Name.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase));
                });
            }

            applications = applications.OrderByDescending(x => x.CreatedAt);

            return PaginationHelper.Process<Application, ApplicationDto>(applications, _page, _pageSize, mapper);
        }
    }

    private record struct IdBasedFilterWithoutNone(long[] IncludeWithId)
    {
        public static bool TryParse(string[] queryParameters, out IdBasedFilterWithoutNone? filter)
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

            var includeWithId = new List<long>();

            foreach (var value in sanitized)
            {
                if (long.TryParse(value, out var longValue))
                {
                    includeWithId.Add(longValue);
                }
                else
                {
                    filter = null;
                    return false;
                }
            }

            filter = new IdBasedFilterWithoutNone(includeWithId.ToArray());
            return true;
        }
    }

    private record struct IdBasedFilterWithNone(bool IncludeWithNone, long[] IncludeWithId)
    {
        public static bool TryParse(string[] queryParameters, out IdBasedFilterWithNone? filter)
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

            var includeWithNone = false;
            var includeWithId = new List<long>();

            foreach (var value in sanitized)
            {
                if (value.Equals("none", StringComparison.Ordinal))
                {
                    includeWithNone = true;
                }
                else if (long.TryParse(value, out var longValue))
                {
                    includeWithId.Add(longValue);
                }
                else
                {
                    filter = null;
                    return false;
                }
            }

            filter = new IdBasedFilterWithNone(includeWithNone, includeWithId.ToArray());
            return true;
        }
    }
}
