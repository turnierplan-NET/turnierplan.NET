using Microsoft.AspNetCore.Mvc;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Pages.Shared.Components.Header;

public sealed class Header : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(Turnierplan.Core.Tournament.Tournament tournament, bool showKickoffAndVenue, bool allowVenueExpand)
    {
        return Task.FromResult<IViewComponentResult>(View(new DataModel(tournament, tournament.PresentationConfiguration.Header1, tournament.PresentationConfiguration.Header2, showKickoffAndVenue, allowVenueExpand)));
    }

    public sealed record DataModel(Turnierplan.Core.Tournament.Tournament Tournament, PresentationConfiguration.HeaderLine Header1, PresentationConfiguration.HeaderLine Header2, bool ShowKickoffAndVenue, bool AllowVenueExpand);
}
