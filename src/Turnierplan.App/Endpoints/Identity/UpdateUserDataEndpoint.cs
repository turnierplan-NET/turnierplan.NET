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
        await IdentityDelay().ConfigureAwait(false);

        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await userRepository.GetByIdAsync(context.GetCurrentUserIdOrThrow()).ConfigureAwait(false);

        if (user is null)
        {
            return Results.Unauthorized();
        }

        user.Name = request.UserName;

        if (!user.NormalizedEMail.Equals(User.NormalizeEmail(request.EMail)))
        {
            // Check if the email address is already taken

            var existingUser = await userRepository.GetByEmailAsync(request.EMail).ConfigureAwait(false);

            if (existingUser is not null)
            {
                return Results.Ok(new UpdateUserDataEndpointResponse
                {
                    Success = false
                });
            }

            user.UpdateEmail(request.EMail);
        }

        await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Give the user a new
        //  - access token which includes the updated username & email claims
        //  - refresh token because the one he currently holds is invalidated due to the updated security stamp
        var accessToken = CreateTokenForUser(user, false);
        var refreshToken = CreateTokenForUser(user, true);

        AddResponseCookieForToken(context, accessToken, false);
        AddResponseCookieForToken(context, refreshToken, true);

        return Results.Ok(new UpdateUserDataEndpointResponse
        {
            Success = true
        });
    }

    public sealed record UpdateUserDataEndpointRequest
    {
        public required string UserName { get; init; }

        public required string EMail { get; init; }
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
            RuleFor(x => x.UserName)
                .NotEmpty();

            RuleFor(x => x.EMail)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
