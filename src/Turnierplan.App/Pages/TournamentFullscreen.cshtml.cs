using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Turnierplan.Core.PublicId;
using Turnierplan.Dal.Repositories;

namespace Turnierplan.App.Pages;

public sealed class TournamentFullscreen : PageModel
{
    private readonly ITournamentRepository _repository;

    public TournamentFullscreen(ITournamentRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? AutoReload { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? AutoScroll { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? ShowQrCode { get; set; }

    public Turnierplan.Core.Tournament.Tournament? Data { get; private set; }

    /// <summary>
    /// The ID of the match which should be scrolled into view upon page load.
    /// </summary>
    public int? ScrollMatchIntoView { get; private set; }

    public async Task OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return;
        }

        PublicId publicId;

        try
        {
            publicId = new PublicId(Id);
        }
        catch
        {
            return;
        }

        var tournament = await _repository.GetByPublicIdAsync(publicId, ITournamentRepository.Includes.GameRelevant | ITournamentRepository.Includes.Images);

        if (tournament is null || !tournament.IsPublic)
        {
            return;
        }

        tournament.Compute();

        Data = tournament;

        ScrollMatchIntoView = AutoScroll == true ? FindMatchToScrollIntoView(tournament) : null;
    }

    private static int? FindMatchToScrollIntoView(Core.Tournament.Tournament tournament)
    {
        var orderedMatches = tournament.Matches.OrderBy(x => x.Index).ToArray();

        for (var i = orderedMatches.Length - 1; i >= 0; i--)
        {
            var match = orderedMatches[i];

            if (match.IsFinished)
            {
                var scrollToMatch = Math.Min(i + 4, orderedMatches.Length - 1);
                return orderedMatches[scrollToMatch].Index;
            }
        }

        return null;
    }
}
