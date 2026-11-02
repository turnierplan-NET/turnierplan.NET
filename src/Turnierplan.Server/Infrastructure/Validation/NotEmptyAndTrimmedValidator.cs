using FluentValidation;
using FluentValidation.Validators;

namespace Turnierplan.Server.Infrastructure.Validation;

internal sealed class NotEmptyAndTrimmedValidator<T> : PropertyValidator<T, string?>, INotEmptyValidator
{
    public override string Name => nameof(NotEmptyAndTrimmedValidator<>);

    public override bool IsValid(ValidationContext<T> context, string? value)
    {
        if (value == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return value.Trim().Length == value.Length;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "'{PropertyName}' must not be empty and must not have whitespace at the start or end.";// Localized(errorCode, Name);
    }
}
