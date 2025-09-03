using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Turnierplan.App.Security;

namespace Turnierplan.App.Endpoints;

internal abstract class EndpointBase<TResponse> : EndpointBase
{
    protected override void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.Produces<TResponse>();
    }
}

internal abstract partial class EndpointBase
{
    protected abstract HttpMethod Method { get; }

    [StringSyntax("Route")]
    protected abstract string Route { get; }

    protected abstract Delegate Handler { get; }

    protected virtual bool DisableAuthorization => false;

    protected virtual bool? AllowApiKeyAccess => null;

    protected virtual bool? RequireAdministrator => null;

    public void Map(IEndpointRouteBuilder endpoints)
    {
        var name = EndpointTypeSuffix().Replace(GetType().Name, string.Empty);
        var builder = endpoints
            .MapMethods(Route, [Method.Method], Handler)
            .DisableAntiforgery()
            .WithName(name)
            .WithSummary(FormatSummary(name))
            .WithTags(NamespacePrefix().Replace(GetType().Namespace ?? string.Empty, string.Empty));

        ConfigureAuthorization(builder);
        ConfigureMetadata(builder);
    }

    protected virtual void ConfigureMetadata(RouteHandlerBuilder builder)
    {
        builder.Produces(204);
    }

    private void ConfigureAuthorization(RouteHandlerBuilder builder)
    {
        if (DisableAuthorization)
        {
            if (RequireAdministrator is not null)
            {
                throw new InvalidOperationException($"Cannot define {nameof(RequireAdministrator)} when {nameof(DisableAuthorization)} is {true}.");
            }

            if (AllowApiKeyAccess.HasValue)
            {
                throw new InvalidOperationException($"Cannot define {nameof(AllowApiKeyAccess)} when {nameof(DisableAuthorization)} is {true}.");
            }
        }
        else
        {
            builder.RequireAuthorization(policy =>
            {
                policy.AuthenticationSchemes = AllowApiKeyAccess is true
                    ? [AuthenticationSchemes.AuthenticationSchemeApiKey, AuthenticationSchemes.AuthenticationSchemeSession]
                    : [AuthenticationSchemes.AuthenticationSchemeSession];

                policy.RequireClaim(ClaimTypes.PrincipalId);

                if (RequireAdministrator == true)
                {
                    policy.RequireClaim(ClaimTypes.Administrator, "true");
                }
            });
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
