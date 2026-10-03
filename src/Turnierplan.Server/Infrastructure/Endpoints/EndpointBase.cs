using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Turnierplan.Server.Infrastructure.Endpoints;

internal abstract partial class EndpointBase
{
    private readonly HttpMethod _method;
    private readonly string _route;
    private readonly string _policy;
    private readonly Type? _responseType;

    protected EndpointBase(HttpMethod method, string route, string policy, Type? responseType = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route);
        ArgumentException.ThrowIfNullOrWhiteSpace(policy);

        _method = method;
        _route = route;
        _policy = policy;
        _responseType = responseType;
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

        var builder = endpoints.MapMethods(_route, [_method.ToString()], handleDelegate)
            .RequireAuthorization(_policy)
            .AddEndpointFilter<ValidationEndpointFilter>()
            .DisableAntiforgery()
            .WithName(name)
            .WithSummary(FormatSummary(name))
            .WithTags(NamespacePrefix().Replace(GetType().Namespace ?? string.Empty, string.Empty));

        if (_responseType is null)
        {
            builder.Produces(204);
        }
        else
        {
            builder.Produces(200, _responseType);
        }
    }


    private static string FormatSummary(string name)
    {
        var result = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]))
            {
                result.Append(' ');
            }

            result.Append(name[i]);
        }

        return result.ToString();
    }

    [GeneratedRegex("Endpoint$")]
    private static partial Regex EndpointTypeSuffix();

    [GeneratedRegex(@"^Turnierplan\.App\.Endpoints\.")]
    private static partial Regex NamespacePrefix();
}
