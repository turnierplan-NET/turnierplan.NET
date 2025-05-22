using FluentValidation;
using FluentValidation.TestHelper;
using Turnierplan.App.Endpoints.Tournaments;

namespace Turnierplan.App.Test.Unit.Endpoints.Tournaments;

public sealed class SetTournamentFolderEndpointTest
{
    public SetTournamentFolderEndpointTest()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }

    [Fact]
    public void SetTournamentFolderEndpointRequest___Without_FolderId_And_FolderName___Is_Valid()
    {
        var command = new SetTournamentFolderEndpoint.SetTournamentFolderEndpointRequest
        {
            FolderId = null,
            FolderName = null
        };

        var result = SetTournamentFolderEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SetTournamentFolderEndpointRequest___With_Only_FolderId___Is_Valid()
    {
        var command = new SetTournamentFolderEndpoint.SetTournamentFolderEndpointRequest
        {
            FolderId = 123,
            FolderName = null
        };

        var result = SetTournamentFolderEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SetTournamentFolderEndpointRequest___With_Only_FolderName___Is_Valid()
    {
        var command = new SetTournamentFolderEndpoint.SetTournamentFolderEndpointRequest
        {
            FolderId = null,
            FolderName = "Test"
        };

        var result = SetTournamentFolderEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void SetTournamentFolderEndpointRequest___With_Both_FolderId_And_FolderName___Is_Invalid()
    {
        var command = new SetTournamentFolderEndpoint.SetTournamentFolderEndpointRequest
        {
            FolderId = 123,
            FolderName = "Test"
        };

        var result = SetTournamentFolderEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Only one of FolderId and FolderName are allowed.")
            .Only();
    }
}
