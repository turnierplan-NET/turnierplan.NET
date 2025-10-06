using System.Text.RegularExpressions;

namespace Turnierplan.App.Constants;

internal static partial class TurnierplanVersion
{
    public static readonly bool IsVersionAvailable;

    public static readonly string Version;

    public static readonly int Major;
    public static readonly int Minor;
    public static readonly int Patch;

    static TurnierplanVersion()
    {
        var assemblyVersion = typeof(TurnierplanVersion).Assembly.GetName().Version?.ToString();

        if (assemblyVersion is null)
        {
            IsVersionAvailable = false;
            Version = "?.?.?";

            return;
        }

        var match = VersionRegex().Match(assemblyVersion);

        if (!match.Success)
        {
            IsVersionAvailable = false;
            Version = "?.?.?";

            return;
        }

        IsVersionAvailable = true;
        Major = int.Parse(match.Groups["Major"].Value);
        Minor = int.Parse(match.Groups["Minor"].Value);
        Patch = int.Parse(match.Groups["Patch"].Value);
        Version = $"{Major}.{Minor}.{Patch}";
    }

    [GeneratedRegex(@"^(?<Major>\d+)\.(?<Minor>\d+)\.(?<Patch>\d+)\.\d+$")]
    private static partial Regex VersionRegex();
}
