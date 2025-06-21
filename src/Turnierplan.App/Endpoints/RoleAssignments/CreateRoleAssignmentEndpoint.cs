using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Helpers;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.ApiKey;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Image;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.SeedWork;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Turnierplan.Core.Venue;
using Turnierplan.Dal;

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
            "Tournament" => CreateRoleAssignmentAsync(request, tournamentRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Tournament>>(), mapper, cancellationToken),
            "Venue" => CreateRoleAssignmentAsync(request, venueRepository, targetId, accessValidator, apiKeyRepository, userRepository, serviceProvider.GetRequiredService<IRoleAssignmentRepository<Venue>>(), mapper, cancellationToken),
            _ => null
        };

        return task is null
            ? Results.BadRequest("Invalid scope identifier provided.")
            : await task.ConfigureAwait(false);
    }

    private static async Task<IResult> CreateRoleAssignmentAsync<TEntity, TIdentifier>(
        CreateRoleAssignmentEndpointRequest request,
        IRepositoryWithPublicId<TEntity, TIdentifier> repository,
        PublicId targetId,
        IAccessValidator accessValidator,
        IApiKeyRepository apiKeyRepository,
        IUserRepository userRepository,
        IRoleAssignmentRepository<TEntity> roleAssignmentRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
        where TEntity : Entity<TIdentifier>, IEntityWithRoleAssignments<TEntity>
    {
        var entity = await repository.GetByPublicIdAsync(targetId).ConfigureAwait(false);

        if (entity is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(entity, Actions.ReadOrWriteRoleAssignments))
        {
            return Results.Forbid();
        }

        var principal = await GetPrincipalAsync(request, apiKeyRepository, userRepository).ConfigureAwait(false);

        if (principal is null)
        {
            return Results.BadRequest("Could not determine principal based on the provided information.");
        }

        if (entity.RoleAssignments.Any(x => x.Role == request.Role && x.Principal.Equals(principal)))
        {
            return Results.Conflict("There already exists a role assignment for the specified principal/role combination.");
        }

        var roleAssignment = entity.AddRoleAssignment(request.Role, principal, request.Description);

        await roleAssignmentRepository.CreateAsync(roleAssignment).ConfigureAwait(false);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<RoleAssignmentDto>(roleAssignment));
    }

    private static async Task<Principal?> GetPrincipalAsync(
        CreateRoleAssignmentEndpointRequest request,
        IApiKeyRepository apiKeyRepository,
        IUserRepository userRepository)
    {
        if (request.ApiKeyId.HasValue)
        {
            var apiKey = await apiKeyRepository.GetByPublicIdAsync(request.ApiKeyId.Value).ConfigureAwait(false);

            return apiKey?.AsPrincipal();
        }

        if (request.UserEmail is not null)
        {
            var user = await userRepository.GetByEmailAsync(request.UserEmail).ConfigureAwait(false);

            return user?.AsPrincipal();
        }

        throw new InvalidOperationException("Invalid request object provided.");
    }

    public sealed record CreateRoleAssignmentEndpointRequest
    {
        public required string ScopeId { get; init; }

        public required Role Role { get; init; }

        public required PublicId? ApiKeyId { get; init; }

        public required string? UserEmail { get; init; }

        public required string Description { get; init; }
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
                .Must(x => x.ApiKeyId is null ^ x.UserEmail is null)
                .WithMessage($"Exactly only one of {nameof(CreateRoleAssignmentEndpointRequest.ApiKeyId)} and {nameof(CreateRoleAssignmentEndpointRequest.UserEmail)} must be specified.");

            RuleFor(x => x.Description)
                .MaximumLength(ValidationConstants.RoleAssignment.MaxDescriptionLength);
        }
    }
}
