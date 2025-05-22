namespace Turnierplan.App.Extensions;

internal static class DateTimeExtensions
{
    public static long GetMillisecondsSinceUtc(this DateTime dateTime)
    {
        if (dateTime.Kind is not DateTimeKind.Utc)
        {
            throw new ArgumentException("Kind of the date time must be UTC.", nameof(dateTime));
        }

        return (dateTime.Ticks - DateTime.UnixEpoch.Ticks) / TimeSpan.TicksPerMillisecond;
    }
}
