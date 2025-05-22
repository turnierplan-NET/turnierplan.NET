using FluentValidation;
using FluentValidation.TestHelper;
using Turnierplan.App.Endpoints.Tournaments;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Test.Unit.Endpoints.Tournaments;

public sealed class CreateTournamentEndpointTest
{
    public CreateTournamentEndpointTest()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }

    [Fact]
    public void CreateTournamentEndpointRequest___Without_FolderId_And_FolderName___Is_Valid()
    {
        var request = new CreateTournamentEndpoint.CreateTournamentEndpointRequest
        {
            OrganizationId = 1,
            FolderId = null,
            FolderName = null,
            Name = "Test",
            Visibility = Visibility.Public
        };

        var result = CreateTournamentEndpoint.Validator.Instance.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateTournamentEndpointRequest___With_Only_FolderId___Is_Valid()
    {
        var request = new CreateTournamentEndpoint.CreateTournamentEndpointRequest
        {
            OrganizationId = 1,
            FolderId = 123,
            FolderName = null,
            Name = "Test",
            Visibility = Visibility.Public
        };

        var result = CreateTournamentEndpoint.Validator.Instance.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateTournamentEndpointRequest___With_Only_FolderName___Is_Valid()
    {
        var request = new CreateTournamentEndpoint.CreateTournamentEndpointRequest
        {
            OrganizationId = 1,
            FolderId = null,
            FolderName = "NewFolder",
            Name = "Test",
            Visibility = Visibility.Public
        };

        var result = CreateTournamentEndpoint.Validator.Instance.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateTournamentEndpointRequest___With_Both_FolderId_And_FolderName___Is_Invalid()
    {
        var request = new CreateTournamentEndpoint.CreateTournamentEndpointRequest
        {
            OrganizationId = 1,
            FolderId = 123,
            FolderName = "NewFolder",
            Name = "Test",
            Visibility = Visibility.Public
        };

        var result = CreateTournamentEndpoint.Validator.Instance.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Only one of FolderId and FolderName are allowed.")
            .Only();
    }
}
