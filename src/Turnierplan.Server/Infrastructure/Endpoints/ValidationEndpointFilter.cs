using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace Turnierplan.Server.Infrastructure.Endpoints;

internal sealed class ValidationEndpointFilter : IEndpointFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationEndpointFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is null)
            {
                continue;
            }

            var argumentType = argument.GetType();

            if (!argumentType.IsClass)
            {
                continue;
            }

            var method = typeof(ValidationEndpointFilter).GetMethod(nameof(ExecuteValidationAsync), BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(argumentType);
            var result = method?.Invoke(this, [argument]);

            if (result is not Task<ValidationResult> validationTask)
            {
                throw new InvalidOperationException($"Failed to execute validation for type '{argumentType.FullName}'.");
            }

            var validationResult = await validationTask;

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        return await next(context);
    }

    private async Task<ValidationResult> ExecuteValidationAsync<T>(T target)
    {
        var hasNoValidationAttribute = typeof(T).GetCustomAttribute<NoValidationAttribute>() is not null;
        var validator = _serviceProvider.GetService<IValidator<T>>();

        if (validator is null)
        {
            return hasNoValidationAttribute
                ? new ValidationResult()
                : throw new InvalidOperationException($"The request object type '{typeof(T).FullName}' has no validator. Add the {nameof(NoValidationAttribute)} if no validation is required.");
        }

        if (hasNoValidationAttribute)
        {
            throw new InvalidOperationException($"The request object type '{typeof(T).FullName}' has both a validator and the {nameof(NoValidationAttribute)} - this indicates a programming mistake.");
        }

        return await validator.ValidateAsync(target);
    }
}
