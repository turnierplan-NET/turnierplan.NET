using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
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
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var user = await repository.GetByIdAsync(id).ConfigureAwait(false);

        if (user is null)
        {
            return Results.NotFound();
        }

        if (!user.NormalizedEMail.Equals(User.NormalizeEmail(request.EMail)))
        {
            // If the email address ought to be changed, check that no other user uses that email address

            var existingUserWithNewEmail = await repository.GetByEmailAsync(request.EMail).ConfigureAwait(false);

            if (existingUserWithNewEmail is not null)
            {
                return Results.BadRequest("The specified email address is already taken.");
            }
        }

        user.Name = request.UserName;

        user.UpdateEmail(request.EMail);

        if (request.UpdatePassword)
        {
            user.UpdatePassword(passwordHasher.HashPassword(user, request.Password!));
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Results.NoContent();
    }

    public sealed record UpdateUserEndpointRequest
    {
        public required string UserName { get; init; }

        public required string EMail { get; init; }

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

            RuleFor(x => x.EMail)
                .EmailAddress();

            RuleFor(x => x.Password)
                .Null()
                .When(x => !x.UpdatePassword);

            RuleFor(x => x.Password)
                .NotEmpty()
                .When(x => x.UpdatePassword);
        }
    }
}
