using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.Core.User;

namespace Turnierplan.App.Endpoints.Users;

internal sealed class UpdateUserEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Put;

    protected override string Route => "/api/users/{id}";

    protected override Delegate Handler => Handle;

    protected override bool? RequireAdministrator => true;

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] UpdateUserEndpointRequest request,
        IPasswordHasher<User> passwordHasher,
        IUserRepository repository,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await repository.GetByIdAsync(id);

        if (user is null)
        {
            return Results.NotFound();
        }

        if (request.EMail is not null && !Equals(user.NormalizedEMail, User.NormalizeEmail(request.EMail)))
        {
            // If the email address ought to be changed, check that no other user uses that email address

            if (await repository.GetByEmailAsync(request.EMail) is not null)
            {
                return Results.BadRequest("The specified email address is already taken.");
            }
        }

        if (httpContext.GetCurrentUserIdOrThrow() == user.Id && !request.IsAdministrator)
        {
            return Results.BadRequest("Cannot take away the administrator privilege of the requesting user.");
        }

        user.UserName = request.UserName.Trim();
        user.FullName = request.FullName?.Trim();
        user.IsAdministrator = request.IsAdministrator;

        user.SetEmailAddress(request.EMail);

        if (request.UpdatePassword)
        {
            user.UpdatePassword(passwordHasher.HashPassword(user, request.Password!));
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    public sealed record UpdateUserEndpointRequest
    {
        public required string UserName { get; init; }

        public string? FullName { get; init; }

        public string? EMail { get; init; }

        public bool IsAdministrator { get; init; }

        public bool UpdatePassword { get; init; }

        public string? Password { get; init; }
    }

    private sealed class Validator : AbstractValidator<UpdateUserEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty();

            RuleFor(x => x.FullName)
                .NotEmpty()
                .When(x => x.FullName is not null);

            RuleFor(x => x.EMail)
                .NotEmpty()
                .EmailAddress()
                .When(x => x.EMail is not null);

            RuleFor(x => x.Password)
                .Null()
                .When(x => !x.UpdatePassword);

            RuleFor(x => x.Password)
                .NotEmpty()
                .When(x => x.UpdatePassword);
        }
    }
}
