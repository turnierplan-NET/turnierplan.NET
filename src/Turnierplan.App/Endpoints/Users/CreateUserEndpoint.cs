using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Turnierplan.App.Extensions;
using Turnierplan.App.Mapping;
using Turnierplan.App.Models;
using Turnierplan.Core.User;

namespace Turnierplan.App.Endpoints.Users;

internal sealed class CreateUserEndpoint : EndpointBase
{
    protected override HttpMethod Method => HttpMethod.Post;

    protected override string Route => "/api/users";

    protected override Delegate Handler => Handle;

    protected override bool? RequireAdministrator => true;

    private static async Task<IResult> Handle(
        [FromBody] CreateUserEndpointRequest request,
        IPasswordHasher<User> passwordHasher,
        IUserRepository repository,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        if (!Validator.Instance.ValidateAndGetResult(request, out var result))
        {
            return result;
        }

        var existingUser = await repository.GetByEmailAsync(request.EMail);

        if (existingUser is not null)
        {
            return Results.BadRequest("The specified email address is already taken.");
        }

        var user = new User(request.UserName, request.EMail);

        user.UpdatePassword(passwordHasher.HashPassword(user, request.Password));

        await repository.CreateAsync(user);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Results.Ok(mapper.Map<UserDto>(user));
    }

    public sealed record CreateUserEndpointRequest
    {
        public required string UserName { get; init; }

        public required string EMail { get; init; }

        public required string Password { get; init; }
    }

    private sealed class Validator : AbstractValidator<CreateUserEndpointRequest>
    {
        public static readonly Validator Instance = new();

        private Validator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty();

            RuleFor(x => x.EMail)
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
