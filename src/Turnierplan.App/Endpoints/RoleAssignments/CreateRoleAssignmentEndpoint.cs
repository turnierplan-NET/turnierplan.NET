using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Entity;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Venue;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Endpoints.RoleAssignments;

internal sealed class CreateRoleAssignmentEndpoint : EndpointBase<RoleAssignmentDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/role-assignments";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateRoleAssignmentEndpointRequest request,
        IApiKeyRepository apiKeyRepository,
        IFolderRepository folderRepository,
        IImageRepository imageRepository,
        IOrganizationRepository organizationRepository,
        IPlanningRealmRepository planningRealmRepository,
        ITournamentRepository tournamentRepository,
        IUserRepository userRepository,
        IVenueRepository venueRepository,
        IAccessValidator accessValidator,
        IServiceProvider serviceProvider,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        if (!RbacScopeHelper.TryParseScopeId(request.ScopeId, out var typeName, out var targetId))
        {
            return Results.BadRequest("Invalid scope identifier provided.");
        }

        var task = typeName switch
        {
            "ApiKey" => CreateRoleAssignmentAsync(request, apiKeyRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<ApiKey>>(), mapper, cancellationToken),
            "Folder" => CreateRoleAssignmentAsync(request, folderRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Folder>>(), mapper, cancellationToken),
            "Image" => CreateRoleAssignmentAsync(request, imageRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Image>>(), mapper, cancellationToken),
            "Organization" => CreateRoleAssignmentAsync(request, organizationRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Organization>>(), mapper, cancellationToken),
            "PlanningRealm" => CreateRoleAssignmentAsync(request, planningRealmRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<PlanningRealm>>(), mapper, cancellationToken),
            "Tournament" => CreateRoleAssignmentAsync(request, tournamentRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Tournament>>(), mapper, cancellationToken),
            "Venue" => CreateRoleAssignmentAsync(request, venueRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Venue>>(), mapper, cancellationToken),
            _ => null
        };

        return task is null
            ? Results.BadRequest("Invalid scope identifier provided.")
            : await task;
    }

    private static async Task<IResult> CreateRoleAssignmentAsync<T>(
        CreateRoleAssignmentEndpointRequest request,
        IRepositoryWithPublicId<T, long> repository,
        PublicId targetId,
        IAccessValidator accessValidator,
        IApiKeyRepository apiKeyRepository,
        IUserRepository userRepository,
        IRoleAssignmentRepository<T> roleAssignmentRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
        where T : Entity<long>, IEntityWithRoleAssignments<T>
    {
        var entity = await repository.GetByPublicIdAsync(targetId);

        if (entity is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(entity, Actions.ReadOrWriteRoleAssignments))
        {
            return Results.Forbid();
        }

        var principal = await GetPrincipalAsync(request, apiKeyRepository, userRepository);

        if (principal is null)
        {
            return Results.BadRequest("Could not determine principal based on the provided information.");
        }

        if (entity.RoleAssignments.Any(x => x.Role == request.Role && x.Principal.Equals(principal)))
        {
            return Results.Conflict("There already exists a role assignment for the specified principal/role combination.");
        }

        var roleAssignment = entity.AddRoleAssignment(request.Role, principal);

        await roleAssignmentRepository.CreateAsync(roleAssignment);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(mapper.Map<RoleAssignmentDto>(roleAssignment));
    }

    private static async Task<Principal?> GetPrincipalAsync(
        CreateRoleAssignmentEndpointRequest request,
        IApiKeyRepository apiKeyRepository,
        IUserRepository userRepository)
    {
        if (request.ApiKeyId.HasValue)
        {
            var apiKey = await apiKeyRepository.GetByPublicIdAsync(request.ApiKeyId.Value);

            return apiKey?.AsPrincipal();
        }

        if (request.UserNameOrEmail is not null)
        {
            var user = await userRepository.GetByUserNameOrEmailAsync(request.UserNameOrEmail);

            return user?.AsPrincipal();
        }

        throw new InvalidOperationException("Invalid request object provided.");
    }

    public sealed record CreateRoleAssignmentEndpointRequest
    {
        public required string ScopeId { get; init; }

        public required Role Role { get; init; }

        public PublicId? ApiKeyId { get; init; }

        public string? UserNameOrEmail { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateRoleAssignmentEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.ScopeId)
                .Matches(RbacScopeHelper.ScopeIdRegex());

            RuleFor(x => x.Role)
                .IsInEnum();

            RuleFor(x => x)
                .Must(x => x.ApiKeyId is null ^ x.UserNameOrEmail is null)
                .WithMessage($"Exactly only one of {nameof(CreateRoleAssignmentEndpointRequest.ApiKeyId)} and {nameof(CreateRoleAssignmentEndpointRequest.UserNameOrEmail)} must be specified.");
        }
    }
}
