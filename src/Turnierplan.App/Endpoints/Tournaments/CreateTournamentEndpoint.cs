using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Extensions;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.User;
using Turnierplan.Dal;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class CreateTournamentEndpoint : EndpointBase<TournamentDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/tournaments";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateTournamentEndpointRequest request,
        HttpContext httpContext,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IAccessValidator accessValidator,
        IFolderRepository folderRepository,
        ITournamentRepository tournamentRepository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await userRepository.GetByIdAsync(httpContext.GetCurrentUserIdOrThrow()).ConfigureAwait(false);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        var queryDetails = request.FolderId is not null ? IOrganizationRepository.Include.Folders : IOrganizationRepository.Include.None;
        var organization = await organizationRepository.GetByPublicIdAsync(request.OrganizationId, queryDetails).ConfigureAwait(false);

        if (organization is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.IsActionAllowed(organization, Actions.GenericWrite))
        {
            return Results.Forbid();
        }

        Folder? folder = null;

        if (request.FolderId.HasValue)
        {
            folder = organization.Folders.SingleOrDefault(x => x.PublicId == request.FolderId.Value);

            if (folder is null)
            {
                return Results.NotFound("The specified folder does not exist.");
            }
        }
        else if (request.FolderName is not null)
        {
            folder = new Folder(organization, request.FolderName);
            folder.AddRoleAssignment(Role.Owner, user.AsPrincipal());

            await folderRepository.CreateAsync(folder).ConfigureAwait(false);
        }

        var tournament = new Tournament(organization, request.Name.Trim(), request.Visibility);
        tournament.AddRoleAssignment(Role.Owner, user.AsPrincipal());
        tournament.SetFolder(folder);

        await tournamentRepository.CreateAsync(tournament).ConfigureAwait(false);
        await tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.Ok(mapper.Map<TournamentDto>(tournament));
    }

    public sealed record CreateTournamentEndpointRequest
    {
        public required PublicId OrganizationId { get; init; }

        public PublicId? FolderId { get; init; }

        public string? FolderName { get; init; }

        public required string Name { get; init; }

        public required Visibility Visibility { get; init; }
    }

    internal sealed class Validator : AbstractValidator<CreateTournamentEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(ValidationConstants.Tournament.MaxNameLength);

            RuleFor(x => x)
                .Must(x => x.FolderId is null ^ x.FolderName is null)
                .WithMessage($"Only one of {nameof(CreateTournamentEndpointRequest.FolderId)} and {nameof(CreateTournamentEndpointRequest.FolderName)} are allowed.")
                .When(x => x.FolderId is not null || x.FolderName is not null);

            RuleFor(x => x.FolderName)
                .NotEmpty()
                .MaximumLength(ValidationConstants.Folder.MaxNameLength)
                .When(x => x.FolderName is not null);

            RuleFor(x => x.Visibility)
                .IsInEnum();
        }
    }
}
