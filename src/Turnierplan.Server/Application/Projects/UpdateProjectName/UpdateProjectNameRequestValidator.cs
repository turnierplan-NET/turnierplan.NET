using FluentValidation;
using Turnierplan.Server.Infrastructure.Validation;

namespace Turnierplan.Server.Application.Projects.UpdateProjectName;

internal sealed class UpdateProjectNameRequestValidator : AbstractValidator<UpdateProjectNameRequest>
{
    public UpdateProjectNameRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyAndTrimmed();
    }
}
