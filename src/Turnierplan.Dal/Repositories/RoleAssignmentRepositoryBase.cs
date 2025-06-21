using Microsoft.EntityFrameworkCore;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;

namespace Turnierplan.Dal.Repositories;

internal abstract class RoleAssignmentRepositoryBase<T>(TurnierplanContext context, DbSet<RoleAssignment<T>> dbSet) : RepositoryBase<RoleAssignment<T>, Guid>(context, dbSet), IRoleAssignmentRepository<T>
    where T : Entity, IEntityWithRoleAssignments<T>;

internal sealed class ApiKeyRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<ApiKey>(context, context.ApiKeyRoleAssignments);

internal sealed class FolderRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Folder>(context, context.FolderRoleAssignments);

internal sealed class ImageRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Image>(context, context.ImageRoleAssignments);

internal sealed class OrganizationRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Organization>(context, context.OrganizationRoleAssignments);

internal sealed class TournamentRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Tournament>(context, context.TournamentRoleAssignments);

internal sealed class VenueRoleAssignmentRepository(TurnierplanContext context) : RoleAssignmentRepositoryBase<Venue>(context, context.VenueRoleAssignments);
