using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Turnierplan.Server.Infrastructure.OpenApi;

namespace Turnierplan.Server.Infrastructure.Endpoints;

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
            .AddEndpointFilter<ValidationEndpointFilter>()
            .DisableAntiforgery()
            .WithName(name)
            .WithSummary(FormatSummary(name))
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

    [GeneratedRegex(@"^Turnierplan\.Server\.Application\.(\w+)")]
    private static partial Regex NamespacePrefix();

    protected enum SpecialResponseKind
    {
        Csv,
        Pdf
    }
}
