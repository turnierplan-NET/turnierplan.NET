using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using Turnierplan.PdfRendering.Renderer;

namespace Turnierplan.PdfRendering.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddTurnierplanDocumentRendering<TApplicationUrlProvider>(this IServiceCollection services)
        where TApplicationUrlProvider : class, IApplicationUrlProvider
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddSingleton<IApplicationUrlProvider, TApplicationUrlProvider>();
        services.AddSingleton<IDocumentTypeRegistry, DocumentTypeRegistry>();

        var baseType = typeof(IDocumentRenderer);
        var attributeType = typeof(DocumentRendererAttribute);
        var rendererTypes = typeof(ServiceCollectionExtensions).Assembly.GetTypes()
            .Where(x => x.GetCustomAttribute(attributeType) is not null);

        foreach (var rendererType in rendererTypes)
        {
            services.AddTransient(baseType, rendererType);
        }
    }
}
