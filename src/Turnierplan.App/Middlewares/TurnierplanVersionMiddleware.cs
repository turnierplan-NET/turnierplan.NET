using Turnierplan.App.Constants;

namespace Turnierplan.App.Middlewares;

internal sealed class TurnierplanVersionMiddleware(RequestDelegate next)
{
    private const string TurnierplanVersionHeaderName = "X-Turnierplan-Version";

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            httpContext.Response.Headers.Append(TurnierplanVersionHeaderName, TurnierplanVersion.Version);
        }

        await next(httpContext);
    }
}
