using FluentValidation;
using Turnierplan.Server.Infrastructure.Validation;

namespace Turnierplan.Server.Application.Organizations.UpdateOrganizationName;

internal sealed class UpdateOrganizationNameRequestValidator : AbstractValidator<UpdateOrganizationNameRequest>
{
    public UpdateOrganizationNameRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyAndTrimmed();
    }
}
