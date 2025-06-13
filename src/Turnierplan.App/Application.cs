namespace Turnierplan.App;

internal static class Application
{
    public static readonly string Version;

    static Application()
    {
        var assemblyVersion = typeof(Application).Assembly.GetName().Version?.ToString();
        Version = assemblyVersion?[..assemblyVersion.LastIndexOf('.')] ?? "?.?.?";
    }
}
