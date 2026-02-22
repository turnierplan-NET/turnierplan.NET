using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            var user = new User(username)
            {
                IsAdministrator = true
            };

            user.UpdatePassword(scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>().HashPassword(user, password));

            ctx.Users.Add(user);
            ctx.SaveChanges();
        }

        var loginRequest = new HttpRequestMessage(HttpMethod.Post, Routes.Identity.Login())
        {
            Content = JsonContent.Create(new { UserName = username, Password = password})
        };

        Client = _application.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        var loginResponseTask = Client.SendAsync(loginRequest);
        loginResponseTask.Wait();
        var loginResponse = loginResponseTask.Result;
        loginResponse.EnsureSuccessStatusCode();
    }

    public HttpClient Client { get; }

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
