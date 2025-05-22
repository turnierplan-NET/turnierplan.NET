using Microsoft.Extensions.Options;
using Turnierplan.App.Options;
using Turnierplan.App.Security;

namespace Turnierplan.App.Endpoints.Identity;

internal sealed class LogoutEndpoint : IdentityEndpointBase<LogoutEndpoint.LogoutEndpointResponse>
{
    public LogoutEndpoint(IOptionsMonitor<IdentityOptions> options, ISigningKeyProvider signingKeyProvider)
        : base(options, signingKeyProvider)
    {
    }

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/identity/logout";

    protected override Delegate Handler => Handle;

    protected override bool DisableAuthorization => true;

    private IResult Handle(HttpContext context)
    {
        AddResponseCookieForToken(context, Guid.Empty.ToString(), false);
        AddResponseCookieForToken(context, Guid.Empty.ToString(), true);

        return Results.Ok(new LogoutEndpointResponse
        {
            Success = true
        });
    }

    public sealed record LogoutEndpointResponse
    {
        public required bool Success { get; init; }
    }
}
