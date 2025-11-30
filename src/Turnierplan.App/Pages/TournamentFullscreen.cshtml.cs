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
    public bool? ShowQrCode { get; set; }

    public Turnierplan.Core.Tournament.Tournament? Data { get; private set; }

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
    }
}
