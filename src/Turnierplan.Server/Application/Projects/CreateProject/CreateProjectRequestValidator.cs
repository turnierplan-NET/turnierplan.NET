using FluentValidation;
using Turnierplan.Server.Infrastructure.Validation;

namespace Turnierplan.Server.Application.Projects.CreateProject;

internal sealed class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmptyAndTrimmed();
    }
}
