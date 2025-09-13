using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Turnierplan.Core.PublicId;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Pages;

public sealed class Tournament : PageModel
{
    private readonly ITournamentRepository _repository;

    public Tournament(ITournamentRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

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

        const ITournamentRepository.Includes includes = ITournamentRepository.Includes.GameRelevant | ITournamentRepository.Includes.Images | ITournamentRepository.Includes.Venue;
        var tournament = await _repository.GetByPublicIdAsync(publicId, includes);

        if (tournament is null || !tournament.IsPublic)
        {
            return;
        }

        tournament.IncrementPublicPageViews();
        await _repository.UnitOfWork.SaveChangesAsync();

        tournament.Compute();

        Data = tournament;
    }
}
