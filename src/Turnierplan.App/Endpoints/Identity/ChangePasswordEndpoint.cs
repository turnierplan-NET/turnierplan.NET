using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Turnierplan.App.Extensions;
using Turnierplan.App.Security;
using Turnierplan.Core.User;
using IdentityOptions = Turnierplan.App.Options.IdentityOptions;

namespace Turnierplan.App.Endpoints.Identity;

internal sealed class ChangePasswordEndpoint : IdentityEndpointBase<ChangePasswordEndpoint.ChangePasswordEndpointResponse>
{
    public ChangePasswordEndpoint(IOptionsMonitor<IdentityOptions> options, ISigningKeyProvider signingKeyProvider)
        : base(options, signingKeyProvider)
    {
    }

    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/identity/change-password";

    protected override Delegate Handler => Handle;

    protected override bool DisableAuthorization => true;

    private async Task<IResult> Handle(
        [FromBody] ChangePasswordEndpointRequest request,
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

        if (IsPasswordInsecure(request.NewPassword))
        {
            return Results.Ok(new ChangePasswordEndpointResponse
            {
                Success = false,
                Reason = ChangePasswordFailedReason.InsecurePassword
            });
        }

        // If user provides current password, and it equals the new password, the request is invalid
        if (request.CurrentPassword.Equals(request.NewPassword))
        {
            return Results.Ok(new ChangePasswordEndpointResponse
            {
                Success = false,
                Reason = ChangePasswordFailedReason.NewPasswordEqualsCurrent
            });
        }

        var user = await userRepository.GetByEmailAsync(request.EMail).ConfigureAwait(false);

        if (user is null)
        {
            return Results.Ok(new ChangePasswordEndpointResponse
            {
                Success = false,
                Reason = ChangePasswordFailedReason.InvalidCredentials
            });
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Results.Ok(new ChangePasswordEndpointResponse
            {
                Success = false,
                Reason = ChangePasswordFailedReason.InvalidCredentials
            });
        }

        user.UpdatePassword(passwordHasher.HashPassword(user, request.NewPassword));

        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Give the user a new refresh token since the one he currently
        // holds is invalidated due to the updated security stamp.
        var refreshToken = CreateTokenForUser(user, true);
        AddResponseCookieForToken(context, refreshToken, true);

        return Results.Ok(new ChangePasswordEndpointResponse
        {
            Success = true,
            RefreshToken = refreshToken
        });
    }

    public sealed record ChangePasswordEndpointRequest
    {
        public required string EMail { get; init; }

        public required string CurrentPassword { get; init; }

        public required string NewPassword { get; init; }
    }

    public sealed record ChangePasswordEndpointResponse
    {
        public required bool Success { get; init; }

        public string? RefreshToken { get; init; }

        public ChangePasswordFailedReason? Reason { get; init; }
    }

    public enum ChangePasswordFailedReason
    {
        InvalidCredentials,
        NewPasswordEqualsCurrent,
        InsecurePassword
    }

    private sealed class Validator : AbstractValidator<ChangePasswordEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.EMail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty();
        }
    }
}
