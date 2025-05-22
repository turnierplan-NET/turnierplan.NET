using FluentValidation;
using FluentValidation.TestHelper;
using Turnierplan.App.Endpoints.Tournaments;
using Turnierplan.App.Models;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Test.Unit.Endpoints.Tournaments;

public sealed class SetTournamentPresentationConfigurationEndpointTest
{
    private static readonly PresentationConfigurationDto __validConfiguration = new()
    {
        Header1 = new PresentationConfigurationDto.HeaderLine
        {
            Content = PresentationConfiguration.HeaderLineContent.TournamentName,
            CustomContent = null
        },
        Header2 = new PresentationConfigurationDto.HeaderLine
        {
            Content = PresentationConfiguration.HeaderLineContent.OrganizerName,
            CustomContent = null
        },
        ShowResults = PresentationConfiguration.ResultsMode.Default,
        ShowOrganizerLogo = true,
        ShowSponsorLogo = true
    };

    public SetTournamentPresentationConfigurationEndpointTest()
    {
        ValidatorOptions.Global.LanguageManager.Enabled = false;
    }

    [Fact]
    public void SetTournamentPresentationConfigurationEndpointRequest___With_Valid_Properties___Is_Valid()
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_Header_1_Is_Custom_But_Custom_Value_Is_Empty___Is_Invalid(string? value)
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                Header1 = new PresentationConfigurationDto.HeaderLine
                {
                    Content = PresentationConfiguration.HeaderLineContent.CustomValue,
                    CustomContent = value
                }
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.Header1.CustomContent)
            .WithErrorMessage("Custom content of header 1 must not be empty if content is 'CustomValue'.")
            .Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_Header_2_Is_Custom_But_Custom_Value_Is_Empty___Is_Invalid(string? value)
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                Header2 = new PresentationConfigurationDto.HeaderLine
                {
                    Content = PresentationConfiguration.HeaderLineContent.CustomValue,
                    CustomContent = value
                }
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.Header2.CustomContent)
            .WithErrorMessage("Custom content of header 2 must not be empty if content is 'CustomValue'.")
            .Only();
    }

    [Fact]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_Header_1_Is_Custom_But_Custom_Value_Is_Too_Long___Is_Invalid()
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                Header1 = new PresentationConfigurationDto.HeaderLine
                {
                    Content = PresentationConfiguration.HeaderLineContent.CustomValue,
                    CustomContent = "012345678901234567890123456789012345678901234567890123456789_"
                }
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.Header1.CustomContent)
            .WithErrorMessage("The length of 'Configuration Header1 Custom Content' must be 60 characters or fewer. You entered 61 characters.")
            .Only();
    }

    [Fact]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_Header_2_Is_Custom_But_Custom_Value_Is_Too_Long___Is_Invalid()
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                Header2 = new PresentationConfigurationDto.HeaderLine
                {
                    Content = PresentationConfiguration.HeaderLineContent.CustomValue,
                    CustomContent = "012345678901234567890123456789012345678901234567890123456789_"
                }
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.Header2.CustomContent)
            .WithErrorMessage("The length of 'Configuration Header2 Custom Content' must be 60 characters or fewer. You entered 61 characters.")
            .Only();
    }

    [Fact]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_Header_1_Has_Invalid_Type___Is_Invalid()
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                Header1 = new PresentationConfigurationDto.HeaderLine
                {
                    Content = (PresentationConfiguration.HeaderLineContent)4
                }
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.Header1.Content)
            .WithErrorMessage("'Configuration Header1 Content' has a range of values which does not include '4'.")
            .Only();
    }

    [Fact]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_Header_2_Has_Invalid_Type___Is_Invalid()
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                Header2 = new PresentationConfigurationDto.HeaderLine
                {
                    Content = (PresentationConfiguration.HeaderLineContent)4
                }
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.Header2.Content)
            .WithErrorMessage("'Configuration Header2 Content' has a range of values which does not include '4'.")
            .Only();
    }

    [Fact]
    public void SetTournamentPresentationConfigurationEndpointRequest___When_ShowResults_Has_Invalid_Type___Is_Invalid()
    {
        var command = new SetTournamentPresentationConfigurationEndpoint.SetTournamentPresentationConfigurationEndpointRequest
        {
            Configuration = __validConfiguration with
            {
                ShowResults = (PresentationConfiguration.ResultsMode)4
            }
        };

        var result = SetTournamentPresentationConfigurationEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Configuration.ShowResults)
            .WithErrorMessage("'Configuration Show Results' has a range of values which does not include '4'.")
            .Only();
    }
}
