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

    public SubmissionState? State { get; private set; }

    public async Task OnGet()
    {
        await LoadInvitationLink().ConfigureAwait(false);
    }

    public async Task OnPost()
    {
        await LoadInvitationLink().ConfigureAwait(false);

        if (Data is null)
        {
            State = SubmissionState.SubmissionFailedBecauseLinkNotFound;
            return;
        }

        if (Data.IsValidUntilSurpassed())
        {
            State = SubmissionState.SubmissionFailedBecauseLinkExpired;
            return;
        }

        var formSessionId = GetFormValue("form_session_id");
        var contactPerson = GetFormValue("contact_person");
        var contactEMail = GetFormValue("contact_email");
        var contactTelephoneNr = GetFormValue("contact_tel");
        var teamName = GetFormValue("team_name");
        var comment = GetFormValue("comment");

        if (!Guid.TryParse(formSessionId, out _) || contactPerson is null || contactEMail is null || teamName is null)
        {
            State = SubmissionState.SubmissionFailedBecauseFormIncomplete;
            return;
        }

        var teamCounts = new Dictionary<InvitationLinkEntry, int>();

        foreach (var entry in Data.Entries)
        {
            var formId = $"team_count_{entry.Id}";
            var formValue = GetFormValue(formId);

            if (formValue is null || !int.TryParse(formValue, out var count) || count == 0)
            {
                continue;
            }

            if (count < 0
                || (entry.MaxTeamsPerRegistration.HasValue && count > entry.MaxTeamsPerRegistration.Value)
                || !entry.AllowNewRegistrations)
            {
                State = SubmissionState.SubmissionFailedBecauseFormTeamsInvalid;
                return;
            }

            teamCounts[entry] = count;
        }

        if (teamCounts.Count == 0)
        {
            State = SubmissionState.SubmissionFailedBecauseFormTeamsEmpty;
            return;
        }

        // TODO: Check that there exists no other application with the same formSessionId

        var application = Data.PlanningRealm.AddApplication(Data, contactPerson);

        application.ContactEmail = contactEMail;
        application.ContactTelephone = contactTelephoneNr;
        application.Comment = comment;

        foreach (var (entry, count) in teamCounts)
        {
            for (var i = 0; i < count; i++)
            {
                var name = count == 1 ? teamName : $"{teamName} {i + 1}";
                application.AddTeam(entry.Class, name);
            }
        }

        await _repository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

        State = SubmissionState.SubmissionSuccessful;
    }

    private async Task LoadInvitationLink()
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

        if (invitationLink is null)
        {
            return;
        }

        // TODO Add condition that checks whether invitationLink is "deactivated" (refer to issue #3)

        Data = invitationLink;
    }

    private string? GetFormValue(string id)
    {
        if (!Request.Form.TryGetValue(id, out var values))
        {
            return null;
        }

        var value = values.FirstOrDefault();

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public enum SubmissionState
    {
        SubmissionSuccessful,
        SubmissionFailedBecauseLinkNotFound,
        SubmissionFailedBecauseLinkExpired,
        SubmissionFailedBecauseFormIncomplete,
        SubmissionFailedBecauseFormTeamsInvalid,
        SubmissionFailedBecauseFormTeamsEmpty
    }
}
