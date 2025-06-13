namespace Turnierplan.App.Constants;

internal static class TurnierplanMetadata
{
    public static readonly string Version = DetermineVersion();

    private static string DetermineVersion()
    {
        var assemblyVersion = typeof(TurnierplanMetadata).Assembly.GetName().Version?.ToString();

        return assemblyVersion?[..assemblyVersion.LastIndexOf('.')] ?? "?.?.?";
    }
}
