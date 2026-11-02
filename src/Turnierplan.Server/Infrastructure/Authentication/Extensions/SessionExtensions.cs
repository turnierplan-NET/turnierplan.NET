using System.Diagnostics.CodeAnalysis;
using Turnierplan.Server.Domain.Users;

namespace Turnierplan.Server.Infrastructure.Authentication.Extensions;

internal static class SessionExtensions
{
    public static void SetUser(this ISession session, User user)
    {
        session.Set(SessionKeys.UserId, BitConverter.GetBytes(user.Id));
    }

    public static void ResetUser(this ISession session)
    {
        session.Remove(SessionKeys.UserId);
    }

    public static bool TryGetUserId(this ISession session, [NotNullWhen(true)] out long? userId)
    {
        if (!session.TryGetValue(SessionKeys.UserId, out var data))
        {
            userId = null;
            return false;
        }

        if (data.Length != 8)
        {
            throw new InvalidOperationException("Session contains unexpected data for user ID key.");
        }

        userId = BitConverter.ToInt64(data);
        return true;
    }

    private static class SessionKeys
    {
        public const string UserId = $"turnierplan_{nameof(UserId)}";
    }
}
