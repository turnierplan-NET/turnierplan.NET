using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.App.Security;
using Turnierplan.Core.Folder;
using Turnierplan.Core.Organization;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Endpoints.Tournaments;

internal sealed class CreateTournamentEndpoint : EndpointBase<TournamentDto>
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/tournaments";

    protected override Delegate Handler => Handle;

    private static async Task<IResult> Handle(
        [FromBody] CreateTournamentEndpointRequest request,
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

        var queryDetails = request.FolderId is not null ? IOrganizationRepository.Includes.Folders : IOrganizationRepository.Includes.None;
        var organization = await organizationRepository.GetByPublicIdAsync(request.OrganizationId, queryDetails);

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

            await folderRepository.CreateAsync(folder);

            accessValidator.AddRolesToResponseHeader(folder);
        }

        var tournament = new Tournament(organization, request.Name.Trim(), request.Visibility);
        tournament.SetFolder(folder);

        await tournamentRepository.CreateAsync(tournament);
        await tournamentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        accessValidator.AddRolesToResponseHeader(tournament);

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
                .NotEmpty();

            RuleFor(x => x)
                .Must(x => x.FolderId is null ^ x.FolderName is null)
                .WithMessage($"Only one of {nameof(CreateTournamentEndpointRequest.FolderId)} and {nameof(CreateTournamentEndpointRequest.FolderName)} are allowed.")
                .When(x => x.FolderId is not null || x.FolderName is not null);

            RuleFor(x => x.FolderName)
                .NotEmpty()
                .When(x => x.FolderName is not null);

            RuleFor(x => x.Visibility)
                .IsInEnum();
        }
    }
}
