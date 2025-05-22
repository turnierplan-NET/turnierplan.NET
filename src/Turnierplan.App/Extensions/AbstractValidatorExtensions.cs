using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Turnierplan.App.Extensions;

internal static class AbstractValidatorExtensions
{
    public static bool ValidateAndGetResult<T>(this AbstractValidator<T> validator, T target, [NotNullWhen(false)] out IResult? result)
    {
        var validationResult = validator.Validate(target);

        result = validationResult.IsValid ? null : Results.ValidationProblem(validationResult.ToDictionary());

        return validationResult.IsValid;
    }
}
