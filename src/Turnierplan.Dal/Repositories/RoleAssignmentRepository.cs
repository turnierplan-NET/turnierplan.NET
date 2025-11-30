using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Entity;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.Dal.Repositories;

public interface IRoleAssignmentRepository<T> : IRepository<RoleAssignment<T>, Guid>
    where T : Entity<long>, IEntityWithRoleAssignments<T>;

internal abstract class RoleAssignmentRepositoryBase<T>(TurnierplanContext context) : RepositoryBase<RoleAssignment<T>, Guid>(context), IRoleAssignmentRepository<T>
    where T : Entity<long>, IEntityWithRoleAssignments<T>;

internal sealed class ApiKeyRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<ApiKey>(context);

internal sealed class FolderRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Folder>(context);

internal sealed class ImageRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Image>(context);

internal sealed class OrganizationRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Organization>(context);

internal sealed class PlanningRealmRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<PlanningRealm>(context);

internal sealed class TournamentRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Tournament>(context);

internal sealed class VenueRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Venue>(context);
