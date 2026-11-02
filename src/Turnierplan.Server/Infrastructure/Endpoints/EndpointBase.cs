using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Infrastructure.OpenApi;

namespace Turnierplan.Server.Infrastructure.Endpoints;

internal abstract class EndpointBase<T> : EndpointBase
{
    protected EndpointBase(HttpMethod method, string route, string policy)
        : base(method, route, policy, responseType: typeof(T))
    {
    }
}

internal abstract partial class EndpointBase
{
    private const string GlobalRoutePrefix = "/api/v1";

    private readonly HttpMethod _method;
    private readonly string _route;
    private readonly string _policy;
    private readonly Type? _responseType;
    private readonly SpecialResponseKind? _specialResponseKind;

    protected EndpointBase(HttpMethod method, string route, string policy, Type? responseType = null, SpecialResponseKind? specialResponseKind = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentException.ThrowIfNullOrWhiteSpace(policy);

        if (route.Length <= 2 || route[0] != '/' || route[1] == '/')
        {
            throw new ArgumentException("The route must start with a single forward slash.", nameof(route));
        }

        if (responseType is not null & specialResponseKind is not null)
        {
            throw new ArgumentException($"The arguments '{nameof(responseType)}' and '{nameof(specialResponseKind)}' are mutually exclusive.");
        }

        _method = method;
        _route = route;
        _policy = policy;
        _responseType = responseType;
        _specialResponseKind = specialResponseKind;
    }

    public void Map(IEndpointRouteBuilder endpoints)
    {
        const string handleMethodName = "HandleAsync";
        var handleMethod = GetType().GetMethod(handleMethodName, BindingFlags.Public | BindingFlags.Static);

        if (handleMethod is null || handleMethod.IsGenericMethod)
        {
            throw new InvalidOperationException($"Derived classes of {nameof(EndpointBase)} must declare a public static non-generic {handleMethodName}() method.");
        }

        var delegateType = Expression.GetDelegateType([..handleMethod.GetParameters().Select(x => x.ParameterType), handleMethod.ReturnType]);
        var handleDelegate = handleMethod.CreateDelegate(delegateType);

        var name = EndpointTypeSuffix().Replace(GetType().Name, string.Empty);

        var builder = endpoints.MapMethods($"{GlobalRoutePrefix}{_route}", [_method.ToString()], handleDelegate)
            .RequireAuthorization(_policy)
            .AddEndpointFilterFactory(CreatePermissionEndpointFilter)
            .AddEndpointFilterFactory(CreateValidationEndpointFilter)
            .DisableAntiforgery()
            .WithName(name)
            .WithTags(NamespacePrefix().Match(GetType().Namespace!).Groups[1].Value);

        if (_specialResponseKind.HasValue)
        {
            switch (_specialResponseKind)
            {
                case SpecialResponseKind.Csv:
                    builder.Produces<ResponseOperationTransformer.ICsvResponse>();
                    break;
                case SpecialResponseKind.Pdf:
                    builder.Produces<ResponseOperationTransformer.IPdfResponse>();
                    break;
                default:
                    throw new InvalidOperationException($"Encountered an unexpected value '{_specialResponseKind}' for special response kind.");
            }
        }
        else if (_responseType is not null)
        {
            builder.Produces(200, _responseType);
        }
        else
        {
            builder.Produces(204);
        }
    }

    private static EndpointFilterDelegate CreatePermissionEndpointFilter(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        var parameters = context.MethodInfo.GetParameters();
        PermissionEndpointFilter? filter = null;

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            var isPublicId = parameter.ParameterType == typeof(PublicId);
            var attribute = parameter.GetCustomAttribute<PermissionAttribute>();

            if (attribute is null)
            {
                if (isPublicId)
                {
                    throw new InvalidOperationException($"The handler method in '{context.MethodInfo.DeclaringType?.FullName}' has a parameter of type '{nameof(PublicId)}' at index {i} which does not have a '{nameof(PermissionAttribute)}'.");
                }

                continue;
            }

            if (attribute.PropertyName is null)
            {
                // When no property name is specified, the parameter type MUST be a public id.

                if (!isPublicId)
                {
                    throw new InvalidOperationException($"The parameter '{parameter.Name}' of handler method in '{context.MethodInfo.DeclaringType?.FullName}' must be of type '{nameof(PublicId)}' when the '{nameof(PermissionAttribute)}' is specified without a parameter name.");
                }
            }
            else
            {
                // When a property name is specified, the parameter type MUST be a class and
                // contain a property with the specified name that must be of type public id.

                var property = parameter.ParameterType.GetProperty(attribute.PropertyName);

                if (!parameter.ParameterType.IsClass || property is null || property.PropertyType != typeof(PublicId))
                {
                    throw new InvalidOperationException($"The parameter '{parameter.Name}' of handler method in '{context.MethodInfo.DeclaringType?.FullName}' is not a class or does not contain a property of name '{attribute.PropertyName}' and type '{nameof(PublicId)}'.");
                }
            }

            filter ??= new PermissionEndpointFilter();
            filter.AddPermissionCheck(i, attribute.PropertyName, attribute.TargetType, attribute.Permission);
        }

        return filter is not null
            ? invocationContext => filter.InvokeAsync(invocationContext, next)
            : next;
    }

    private static EndpointFilterDelegate CreateValidationEndpointFilter(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        var bodyParameter = context.MethodInfo.GetParameters()
            .SingleOrDefault(param => param.GetCustomAttribute<FromBodyAttribute>() is not null);

        if (bodyParameter is null)
        {
            return next;
        }

        var filterType = typeof(ValidationEndpointFilter<>).MakeGenericType(bodyParameter.ParameterType);
        var instance = ActivatorUtilities.CreateInstance(context.ApplicationServices, filterType);

        if (instance is not IEndpointFilter filter)
        {
            throw new InvalidOperationException($"Failed to instantiate filter class '{filterType.FullName}'.");
        }

        return invocationContext => filter.InvokeAsync(invocationContext, next);
    }

    [GeneratedRegex("Endpoint$")]
    private static partial Regex EndpointTypeSuffix();

    [GeneratedRegex(@"^Turnierplan\.Server\.Application\.(\w+)")]
    private static partial Regex NamespacePrefix();

    protected enum SpecialResponseKind
    {
        Csv,
        Pdf
    }
}
