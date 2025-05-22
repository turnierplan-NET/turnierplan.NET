using Microsoft.Extensions.DependencyInjection;

namespace Turnierplan.Localization.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTurnierplanLocalization(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizationProvider, LocalizationProvider>();
    }
}
