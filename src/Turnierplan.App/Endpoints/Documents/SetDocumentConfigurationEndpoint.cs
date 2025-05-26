using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.Document;
using Turnierplan.Core.PublicId;
using Turnierplan.PdfRendering.Configuration;

namespace Turnierplan.App.Endpoints.Documents;

internal abstract class SetDocumentConfigurationEndpoint<T> : EndpointBase
    where T : IDocumentConfiguration
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly DocumentType _documentType;

    protected SetDocumentConfigurationEndpoint(DocumentType documentType)
    {
        _documentType = documentType;
    }

    protected abstract AbstractValidator<T> Validator { get; }

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => $"/api/documents/{{id}}/{_documentType.ToString().ToSnakeCase()}/configuration";

    protected override Delegate Handler => Handle;

    private async Task<IResult> Handle(
        [FromRoute] PublicId id,
        [FromBody] T configuration,
        IDocumentRepository repository,
        IAccessValidator accessValidator,
        CancellationToken cancellationToken)
    {
        var document = await repository.GetByPublicIdAsync(id).ConfigureAwait(false);

        if (document is null)
        {
            return Results.NotFound();
        }

        if (!accessValidator.CanSessionUserAccess(document.Tournament.Organization))
        {
            return Results.Forbid();
        }

        if (document.Type != _documentType)
        {
            return Results.BadRequest("Specified document configuration does not match the type of the document.");
        }

        if (!Validator.ValidateAndGetResult(configuration, out var result))
        {
            return result;
        }

        document.UpdateConfiguration(JsonSerializer.Serialize(configuration, _jsonSerializerOptions));

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }
}

internal sealed class SetMatchPlanDocumentConfigurationEndpoint : SetDocumentConfigurationEndpoint<MatchPlanDocumentConfiguration>
{
    public SetMatchPlanDocumentConfigurationEndpoint()
        : base(DocumentType.MatchPlan)
    {
    }

    protected override AbstractValidator<MatchPlanDocumentConfiguration> Validator => MatchPlanDocumentConfigurationValidator.Instance;

    internal sealed class MatchPlanDocumentConfigurationValidator : AbstractValidator<MatchPlanDocumentConfiguration>
    {
        public static readonly MatchPlanDocumentConfigurationValidator Instance = new();

        private MatchPlanDocumentConfigurationValidator()
        {
            RuleFor(x => x.OrganizerNameOverride)
                .MaximumLength(100)
                .When(x => x.OrganizerNameOverride is not null);

            RuleFor(x => x.TournamentNameOverride)
                .MaximumLength(100)
                .When(x => x.TournamentNameOverride is not null);

            RuleFor(x => x.VenueOverride)
                .MaximumLength(100)
                .When(x => x.VenueOverride is not null);

            RuleFor(x => x.DateFormat)
                .NotNull()
                .IsInEnum();

            RuleFor(x => x.Outcomes)
                .NotNull()
                .IsInEnum();

            RuleFor(x => x.IncludeRankingTable)
                .Must(x => !x)
                .WithMessage($"{nameof(MatchPlanDocumentConfiguration.IncludeRankingTable)} must be false if {nameof(MatchPlanDocumentConfiguration.Outcomes)} is {nameof(MatchPlanOutcomes.HideOutcomeStructures)}.")
                .When(x => x.Outcomes is MatchPlanOutcomes.HideOutcomeStructures);
        }
    }
}

internal sealed class SetReceiptsDocumentConfigurationEndpoint : SetDocumentConfigurationEndpoint<ReceiptsDocumentConfiguration>
{
    public SetReceiptsDocumentConfigurationEndpoint()
        : base(DocumentType.Receipts)
    {
    }

    protected override AbstractValidator<ReceiptsDocumentConfiguration> Validator => ReceiptsDocumentConfigurationValidator.Instance;

    internal sealed class ReceiptsDocumentConfigurationValidator : AbstractValidator<ReceiptsDocumentConfiguration>
    {
        public static readonly ReceiptsDocumentConfigurationValidator Instance = new();

        private ReceiptsDocumentConfigurationValidator()
        {
            RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
            RuleFor(x => x.HeaderInfo).MaximumLength(100);
            RuleFor(x => x.SignatureLocation).MaximumLength(100);
            RuleFor(x => x.SignatureRecipient).MaximumLength(100);

            RuleFor(x => x.Amounts)
                .Must(x => x.ContainsKey(1))
                .WithMessage("An amount entry for 1 team must always be specified.");

            RuleFor(x => x.Amounts.Count)
                .LessThanOrEqualTo(4)
                .WithMessage("The number of amount entries must be at most 4.");
        }
    }
}
