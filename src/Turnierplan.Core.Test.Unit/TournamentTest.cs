using System.Globalization;
using FluentAssertions;
using FluentAssertions.Extensions;
using Turnierplan.Core.Configuration;
using Turnierplan.Core.TeamSelectors;
using Xunit;

namespace Turnierplan.Core.Test.Unit;

public sealed class TournamentTest
{
    [Theory]
    [InlineData("2022-12-27T08:00:00Z", "Europe/Berlin", "12/27/2022 09:00")]
    [InlineData("2022-07-27T08:00:00Z", "Europe/Berlin", "07/27/2022 10:00")]
    [InlineData("2022-07-27T08:00:00Z", "America/New_York", "07/27/2022 04:00")]
    public void Tournament___ShiftToTimezone___Works_As_Expected(string originalUtcKickoffTime, string timeZoneName, string expectedLocalKickoffTime)
    {
        // Arrange
        var tournament = new Tournament(string.Empty);
        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), null!)
        {
            Kickoff = DateTime.Parse(originalUtcKickoffTime, CultureInfo.InvariantCulture).ToUniversalTime()
        });

        // Act
        tournament.ShiftToTimezone(TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));

        // Assert
        string.Format(CultureInfo.InvariantCulture, "{0:g}", tournament._matches.Single().Kickoff).Should().Be(expectedLocalKickoffTime);
    }

    [Fact]
    public void Tournament___StartTimestamp_And_EndTimestamp_When_Matches_Have_No_Kickoff_Time___Returns_Correct_Value()
    {
        var tournament = new Tournament(string.Empty);
        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(2, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(3, new NullTeamSelector(), new NullTeamSelector(), null!));

        tournament.StartTimestamp.Should().BeNull();
        tournament.EndTimestamp.Should().BeNull();
    }

    [Fact]
    public void Tournament___StartTimestamp_And_EndTimestamp_When_Matches_Have_Kickoff_Time___Returns_Correct_Value()
    {
        var tournament = new Tournament(string.Empty);
        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 00, 00) });
        tournament._matches.Add(new Match(2, new NullTeamSelector(), new NullTeamSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 10, 00) });
        tournament._matches.Add(new Match(3, new NullTeamSelector(), new NullTeamSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 20, 00) });

        tournament.StartTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, 00, 00));
        tournament.EndTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, 20, 00));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Tournament___StartTimestamp_And_EndTimestamp_When_Matches_Have_Kickoff_Time_And_Duration___Returns_Correct_Value(bool isFinalsMatch)
    {
        var tournament = new Tournament(string.Empty)
        {
            MatchPlanConfiguration = new MatchPlanConfiguration
            {
                ScheduleConfig = new ScheduleConfig
                {
                    FirstMatchKickoff = default,
                    GroupPhaseNumberOfCourts = 0,
                    GroupPhasePlayTime = 7.Minutes(),
                    GroupPhasePauseTime = TimeSpan.Zero,
                    PauseBetweenGroupAndFinalsPhase = TimeSpan.Zero,
                    FinalsPhaseNumberOfCourts = 0,
                    FinalsPhasePlayTime = 9.Minutes(),
                    FinalsPhasePauseTime = TimeSpan.Zero
                }
            }
        };

        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 00, 00) });
        tournament._matches.Add(new Match(2, new NullTeamSelector(), new NullTeamSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 10, 00) });

        var group = isFinalsMatch ? null : new Group('A');
        tournament._matches.Add(new Match(3, new NullTeamSelector(), new NullTeamSelector(), group) { Kickoff = new DateTime(2024, 07, 01, 12, 20, 00) });

        tournament.StartTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, 00, 00));
        tournament.EndTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, isFinalsMatch ? 29 : 27, 00));
    }
}
