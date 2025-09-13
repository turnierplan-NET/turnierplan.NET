using FluentValidation.TestHelper;
using Turnierplan.App.Endpoints.Matches;
using Turnierplan.App.Models.Enums;
using Turnierplan.Core.Tournament;

namespace Turnierplan.App.Test.Unit.Endpoints.Matches;

public sealed class SetMatchOutcomeEndpointTest
{
    [Theory]
    [InlineData(0, null, null)]
    [InlineData(null, 0, null)]
    [InlineData(null, null, MatchOutcomeType.Standard)]
    [InlineData(0, 0, null)]
    [InlineData(0, 0, MatchOutcomeType.Standard)]
    public void SetMatchOutcomeEndpointRequest___With_Invalid_Score_When_State_Is_Not_Started___Is_Invalid(int? scoreA, int? scoreB, MatchOutcomeType? outcomeType)
    {
        var command = new SetMatchOutcomeEndpoint.SetMatchOutcomeEndpointRequest
        {
            State = MatchState.NotStarted,
            ScoreA = scoreA,
            ScoreB = scoreB,
            OutcomeType = outcomeType
        };

        var result = SetMatchOutcomeEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrors();

        if (scoreA is not null)
        {
            result.ShouldHaveValidationErrorFor(x => x.ScoreA)
                .WithErrorMessage("ScoreA must be null if State is NotStarted.");
        }

        if (scoreB is not null)
        {
            result.ShouldHaveValidationErrorFor(x => x.ScoreB)
                .WithErrorMessage("ScoreB must be null if State is NotStarted.");
        }

        if (outcomeType is not null)
        {
            result.ShouldHaveValidationErrorFor(x => x.OutcomeType)
                .WithErrorMessage("OutcomeType must be null if State is NotStarted.");
        }
    }

    [Theory]
    [InlineData(null, 0, MatchOutcomeType.Standard, MatchState.CurrentlyPlaying)]
    [InlineData(null, 0, MatchOutcomeType.Standard, MatchState.Finished)]
    [InlineData(0, null, MatchOutcomeType.Standard, MatchState.CurrentlyPlaying)]
    [InlineData(0, null, MatchOutcomeType.Standard, MatchState.Finished)]
    [InlineData(0, 0, null, MatchState.CurrentlyPlaying)]
    [InlineData(0, 0, null, MatchState.Finished)]
    [InlineData(-1, 0, MatchOutcomeType.Standard, MatchState.Finished)]
    [InlineData(0, -1, MatchOutcomeType.Standard, MatchState.Finished)]
    public void SetMatchOutcomeEndpointRequest___With_Invalid_Score_When_State_Is_CurrentlyPlaying_Or_Finished___Is_Invalid(int? scoreA, int? scoreB, MatchOutcomeType? outcomeType, MatchState state)
    {
        var command = new SetMatchOutcomeEndpoint.SetMatchOutcomeEndpointRequest
        {
            State = state,
            ScoreA = scoreA,
            ScoreB = scoreB,
            OutcomeType = outcomeType
        };

        var result = SetMatchOutcomeEndpoint.Validator.Instance.TestValidate(command);

        result.ShouldHaveValidationErrors();

        if (scoreA is null)
        {
            result.ShouldHaveValidationErrorFor(x => x.ScoreA)
                .WithErrorMessage("'Score A' must not be empty.");
        }

        if (scoreA < 0)
        {
            result.ShouldHaveValidationErrorFor(x => x.ScoreA)
                .WithErrorMessage("ScoreA must be not null and non-negative if State is CurrentlyPlaying or Finished.");
        }

        if (scoreB is null)
        {
            result.ShouldHaveValidationErrorFor(x => x.ScoreB)
                .WithErrorMessage("'Score B' must not be empty.");
        }

        if (scoreB < 0)
        {
            result.ShouldHaveValidationErrorFor(x => x.ScoreB)
                .WithErrorMessage("ScoreB must be not null and non-negative if State is CurrentlyPlaying or Finished.");
        }

        if (outcomeType is null)
        {
            result.ShouldHaveValidationErrorFor(x => x.OutcomeType)
                .WithErrorMessage("OutcomeType must be not null if State is CurrentlyPlaying or Finished.");
        }
    }
}
