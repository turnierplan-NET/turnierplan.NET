namespace Turnierplan.App.Constants;

internal static class TurnierplanVersion
{
    public static readonly string Version = DetermineVersion();

    private static string DetermineVersion()
    {
        var assemblyVersion = typeof(TurnierplanVersion).Assembly.GetName().Version?.ToString();

        return assemblyVersion?[..assemblyVersion.LastIndexOf('.')] ?? "?.?.?";
    }
}
