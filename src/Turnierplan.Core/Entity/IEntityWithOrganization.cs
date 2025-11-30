namespace Turnierplan.Core.Entity;

public interface IEntityWithOrganization
{
    Organization.Organization Organization { get; }
}
