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
        IPlanningRealmRepository planningRealmRepository,
        IAccessValidator accessValidator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        var planningRealm = await planningRealmRepository.GetByPublicIdAsync(planningRealmId, IPlanningRealmRepository.Include.TournamentClasses | IPlanningRealmRepository.Include.ApplicationsWithTeams).ConfigureAwait(false);

        if (planningRealm is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(planningRealm, Actions.ManageApplications))
        {
            return Results.Forbid();
        }

        var queryLogic = new QueryLogic(page, pageSize, searchTerm);
        var result = queryLogic.Process(planningRealm, mapper);

        return Results.Ok(result);
    }

    private sealed class QueryLogic
    {
        private const int DefaultPageSize = 20;

        private readonly int _page;
        private readonly int _pageSize;
        private readonly string? _searchTerm;

        public QueryLogic(int? page, int? pageSize, string? searchTerm)
        {
            _page = page ?? 0;
            _pageSize = pageSize ?? DefaultPageSize;
            _searchTerm = searchTerm?.Trim();
        }

        public PaginationResultDto<ApplicationDto> Process(PlanningRealm planningRealm, IMapper mapper)
        {
            var applications = planningRealm.Applications.AsEnumerable();

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

            return PaginationHelper.Process<Application, ApplicationDto>(applications, _page, _pageSize, mapper);
        }
    }
}
