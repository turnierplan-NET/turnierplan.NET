using Turnierplan.Server.Domain.Organizations;

namespace Turnierplan.Server.Domain;

internal interface IInheritsRolesFromOrganization
{
    Organization Organization { get; }
}
