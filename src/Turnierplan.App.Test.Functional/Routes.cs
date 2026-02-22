using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Test.Functional;

internal static class Routes
{
    public static class ApiKeys
    {
        public static string Delete(PublicId id) => $"/api/api-keys/{id}";
    }

    public static class Identity
    {
        public static string Login() => "/api/identity/login";
    }

    public static class Users
    {
        public static string Delete(Guid id) => $"/api/users/{id}";
    }
}
