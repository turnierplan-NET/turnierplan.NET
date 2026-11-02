using FluentValidation;

namespace Turnierplan.Server.Infrastructure.Validation;

internal static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string?> NotEmptyAndTrimmed<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new NotEmptyAndTrimmedValidator<T>());
    }
}
