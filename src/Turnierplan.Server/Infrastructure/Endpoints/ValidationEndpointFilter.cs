using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace Turnierplan.Server.Infrastructure.Endpoints;

// TODO: Add global exception handler to "convert" the exception into a 400 Bad Request
internal sealed class ValidationEndpointFilter<T> : IEndpointFilter
{
    private static readonly Type __targetType = typeof(T);

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arguments = context.Arguments.OfType<T>().ToArray();

        if (arguments.Length != 1)
        {
            throw new InvalidOperationException($"Expected exactly one argument of type '{__targetType.FullName}', but got {arguments.Length}.");
        }

        var hasNoValidationAttribute = __targetType.GetCustomAttribute<NoValidationAttribute>() is not null;
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is null)
        {
            return hasNoValidationAttribute
                ? new ValidationResult()
                : throw new InvalidOperationException($"The request object type '{__targetType.FullName}' has no validator. Add the {nameof(NoValidationAttribute)} if no validation is required.");
        }

        if (hasNoValidationAttribute)
        {
            throw new InvalidOperationException($"The request object type '{__targetType.FullName}' has both a validator and the {nameof(NoValidationAttribute)} - this indicates a programming mistake.");
        }

        var validationResult = await validator.ValidateAsync(arguments[0]);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        return await next(context);
    }
}
