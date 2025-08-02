using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Turnierplan.Core.PlanningRealm;
using Turnierplan.Core.PublicId;

namespace Turnierplan.App.Pages;

public sealed class InvitationForm : PageModel
{
    private readonly IInvitationLinkRepository _repository;

    public InvitationForm(IInvitationLinkRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public string? Id { get; set; }

    public InvitationLink? Data { get; private set; }

    public async Task OnGet()
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

        var invitationLink = await _repository.GetByPublicIdAsync(publicId).ConfigureAwait(false);

        if (invitationLink is null) // TODO Add condition that checks invitationLink is "deactivated" (refer to issue #3)
        {
            return;
        }

        Data = invitationLink;
    }
}
