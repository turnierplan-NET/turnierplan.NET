using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.User;
using IdentityOptions = Turnierplan.App.Options.IdentityOptions;

namespace Turnierplan.App.Endpoints.Identity;

internal sealed class UpdateUserDataEndpoint : IdentityEndpointBase<UpdateUserDataEndpoint.UpdateUserDataEndpointResponse>
{
    public UpdateUserDataEndpoint(IOptionsMonitor<IdentityOptions> options, ISigningKeyProvider signingKeyProvider)
        : base(options, signingKeyProvider)
    {
    }

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/identity/user-data";

    protected override Delegate Handler => Handle;

    private async Task<IResult> Handle(
        [FromBody] UpdateUserDataEndpointRequest request,
        HttpContext context,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await userRepository.GetByIdAsync(context.GetCurrentUserIdOrThrow());

        if (user is null)
        {
            return Results.Unauthorized();
        }

        user.FullName = request.FullName;

        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Give the user a new access token which includes the updated username claim
        var accessToken = CreateTokenForUser(user, false);

        AddResponseCookieForToken(context, accessToken, false);

        return Results.Ok(new UpdateUserDataEndpointResponse
        {
            Success = true
        });
    }

    public sealed record UpdateUserDataEndpointRequest
    {
        public string? FullName { get; init; }
    }

    public sealed record UpdateUserDataEndpointResponse
    {
        public required bool Success { get; init; }
    }

    private sealed class Validator : AbstractValidator<UpdateUserDataEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .When(x => x.FullName is not null);
        }
    }
}
