using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Turnierplan.App.Test.Functional.Client;
using Turnierplan.App.Test.Functional.Client.Models;
using Turnierplan.Core.User;
using Turnierplan.Dal;

namespace Turnierplan.App.Test.Functional;

internal sealed class TestServer
{
    private readonly WebApplicationFactory<Program> _application;

    public TestServer()
    {
        _application = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection([
                    new KeyValuePair<string, string?>("Database:InMemory", "true"),
                    new KeyValuePair<string, string?>("Identity:UseInsecureCookies", "true")
                ]);
            });
        });

        const string username = "functional_test";
        const string password = "P@ssw0rd";

        using (var scope = _application.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<TurnierplanContext>();

            var user = new User(username);

            user.SetIsAdministrator(true);
            user.UpdatePassword(scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>().HashPassword(user, password));

            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        Client = CreateNewClientAndLogInAsync(username, password).GetAwaiter().GetResult();
    }

    public TurnierplanClient Client { get; }

    public async Task<TurnierplanClient> CreateNewClientAndLogInAsync(string username, string password)
    {
        var authenticationProvider = new AnonymousAuthenticationProvider();
        var httpClient = _application.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var httpClientRequestAdapter = new HttpClientRequestAdapter(authenticationProvider, httpClient: httpClient);
        var client = new TurnierplanClient(httpClientRequestAdapter);

        var loginResponse = await client.Api.Identity.Login.PostAsync(new LoginEndpointRequest
        {
            UserName = username,
            Password = password
        });

        loginResponse!.Success.Should().BeTrue();

        return client;
    }

    public void ExecuteContextAction(Action<TurnierplanContext> action)
    {
        using var scope = _application.Services.CreateScope();
        action(scope.ServiceProvider.GetRequiredService<TurnierplanContext>());
    }

    public T ExecuteContextAction<T>(Func<TurnierplanContext, T> action)
    {
        using var scope = _application.Services.CreateScope();
        return action(scope.ServiceProvider.GetRequiredService<TurnierplanContext>());
    }
}
