using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.User;
using IdentityOptions = Turnierplan.App.Options.IdentityOptions;

namespace Turnierplan.App.Endpoints.Identity;

internal sealed class LoginEndpoint : IdentityEndpointBase<LoginEndpoint.LoginEndpointResponse>
{
    public LoginEndpoint(IOptionsMonitor<IdentityOptions> options, ISigningKeyProvider signingKeyProvider)
        : base(options, signingKeyProvider)
    {
    }

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/identity/login";

    protected override Delegate Handler => Handle;

    protected override bool DisableAuthorization => true;

    private async Task<IResult> Handle(
        [FromBody] LoginEndpointRequest request,
        HttpContext context,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        CancellationToken cancellationToken)
    {
        await IdentityDelay().ConfigureAwait(false);

        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await userRepository.GetByEmailAsync(request.EMail).ConfigureAwait(false);

        if (user is null)
        {
            return Results.Ok(new LoginEndpointResponse
            {
                Success = false
            });
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Results.Ok(new LoginEndpointResponse
            {
                Success = false
            });
        }

        user.UpdateLastLoginTime();

        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var accessToken = CreateTokenForUser(user, false);
        var refreshToken = CreateTokenForUser(user, true);

        AddResponseCookieForToken(context, accessToken, false);
        AddResponseCookieForToken(context, refreshToken, true);

        return Results.Ok(new LoginEndpointResponse
        {
            Success = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    public sealed record LoginEndpointRequest
    {
        public required string EMail { get; init; }

        public required string Password { get; init; }
    }

    public sealed record LoginEndpointResponse
    {
        public required bool Success { get; init; }

        public string? AccessToken { get; init; }

        public string? RefreshToken { get; init; }
    }

    private sealed class Validator : AbstractValidator<LoginEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.EMail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
