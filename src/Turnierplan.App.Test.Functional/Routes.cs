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

    public static class Organizations
    {
        public static string Create() => "/api/organizations";
    }

    public static class Users
    {
        public static string List() => "/api/users";
        public static string Create() => "/api/users";
        public static string Delete(Guid id) => $"/api/users/{id}";
        public static string Update(Guid id) => $"/api/users/{id}";
    }
}
