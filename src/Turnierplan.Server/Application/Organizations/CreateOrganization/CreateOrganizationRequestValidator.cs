using FluentValidation;
using Turnierplan.Server.Infrastructure.Validation;

namespace Turnierplan.Server.Application.Organizations.CreateOrganization;

internal sealed class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyAndTrimmed();
    }
}