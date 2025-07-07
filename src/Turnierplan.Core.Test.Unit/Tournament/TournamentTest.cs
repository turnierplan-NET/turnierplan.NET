using System.Globalization;
using Turnierplan.Core.Exceptions;
using Turnierplan.Core.Tournament;
using Turnierplan.Core.Tournament.TeamSelectors;

namespace Turnierplan.Core.Test.Unit.Tournament;

public sealed class TournamentTest
{
    [Theory]
    [InlineData("2022-12-27T08:00:00Z", "Europe/Berlin", "12/27/2022 09:00")]
    [InlineData("2022-07-27T08:00:00Z", "Europe/Berlin", "07/27/2022 10:00")]
    [InlineData("2022-07-27T08:00:00Z", "America/New_York", "07/27/2022 04:00")]
    public void Tournament___ShiftToTimezone___Works_As_Expected(string originalUtcKickoffTime, string timeZoneName, string expectedLocalKickoffTime)
    {
        // Arrange
        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), null!)
        {
            Kickoff = DateTime.Parse(originalUtcKickoffTime).ToUniversalTime()
        });

        // Act
        tournament.ShiftToTimezone(TimeZoneInfo.FindSystemTimeZoneById(timeZoneName));

        // Assert
        string.Format(CultureInfo.InvariantCulture, "{0:g}", tournament._matches.Single().Kickoff).Should().Be(expectedLocalKickoffTime);
    }

    [Fact]
    public void Tournament___SetMatchOrder_With_Valid_Input___Works_As_Expected()
    {
        // Arrange
        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(2, 2, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(3, 3, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(4, 4, new NullSelector(), new NullSelector(), null!));

        // Act
        tournament.SetMatchOrder(new Dictionary<int, int>
        {
            { 1, 4 },
            { 2, 2 },
            { 3, 3 },
            { 4, 1 }
        });

        // Assert
        tournament._matches.Single(x => x.Id == 1).Index.Should().Be(4);
        tournament._matches.Single(x => x.Id == 2).Index.Should().Be(2);
        tournament._matches.Single(x => x.Id == 3).Index.Should().Be(3);
        tournament._matches.Single(x => x.Id == 4).Index.Should().Be(1);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }, "The given match IDs must equal the existing match IDs.")]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 2, 3, 4, 5 }, "The given match IDs must equal the existing match IDs.")]
    [InlineData(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 4, 4 }, "The match indices must be unique.")]
    [InlineData(new[] { 1, 2, 3, 4 }, new[] { 1, 2, 4, 5 }, "The match indices must be sequential, starting from 1.")]
    [InlineData(new[] { 1, 2, 3, 4 }, new[] { 2, 3, 4, 5 }, "The match indices must be sequential, starting from 1.")]
    public void Tournament___SetMatchOrder_With_Invalid_Input___Throws_Exception(int[] matchIds, int[] matchIndices, string expectedExceptionMessage)
    {
        matchIds.Should().HaveSameCount(matchIndices);

        // Arrange
        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(2, 2, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(3, 3, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(4, 4, new NullSelector(), new NullSelector(), null!));

        // Act
        var order = Enumerable.Range(0, matchIds.Length).ToDictionary(i => matchIds[i], i => matchIndices[i]);
        var action = () => tournament.SetMatchOrder(order);

        // Assert
        action.Should().ThrowExactly<TurnierplanException>().WithMessage(expectedExceptionMessage);
    }

    [Fact]
    public void Tournament___StartTimestamp_And_EndTimestamp_When_Matches_Have_No_Kickoff_Time___Returns_Correct_Value()
    {
        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(2, 2, new NullSelector(), new NullSelector(), null!));
        tournament._matches.Add(new Match(3, 3, new NullSelector(), new NullSelector(), null!));

        tournament.StartTimestamp.Should().BeNull();
        tournament.EndTimestamp.Should().BeNull();
    }

    [Fact]
    public void Tournament___StartTimestamp_And_EndTimestamp_When_Matches_Have_Kickoff_Time___Returns_Correct_Value()
    {
        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 00, 00) });
        tournament._matches.Add(new Match(2, 2, new NullSelector(), new NullSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 10, 00) });
        tournament._matches.Add(new Match(3, 3, new NullSelector(), new NullSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 20, 00) });

        tournament.StartTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, 00, 00));
        tournament.EndTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, 20, 00));
    }

    [Theory, CombinatorialData]
    public void Tournament___StartTimestamp_And_EndTimestamp_When_Matches_Have_Kickoff_Time_And_Duration___Returns_Correct_Value(bool isFinalsMatch)
    {
        var tournament = TestTournament.Default;
        tournament.MatchPlanConfiguration = new MatchPlanConfiguration
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
        };

        tournament._matches.Add(new Match(1, 1, new NullSelector(), new NullSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 00, 00) });
        tournament._matches.Add(new Match(2, 2, new NullSelector(), new NullSelector(), null!) { Kickoff = new DateTime(2024, 07, 01, 12, 10, 00) });

        var group = isFinalsMatch ? null : new Group(1, 'A');
        tournament._matches.Add(new Match(3, 3, new NullSelector(), new NullSelector(), group) { Kickoff = new DateTime(2024, 07, 01, 12, 20, 00) });

        tournament.StartTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, 00, 00));
        tournament.EndTimestamp.Should().Be(new DateTime(2024, 07, 01, 12, isFinalsMatch ? 29 : 27, 00));
    }

    [Fact]
    public void Tournament___IncrementPublicPageViews_When_Tournament_Is_Private___Throws_Exception()
    {
        var tournament = TestTournament.Default;
        var action = () => tournament.IncrementPublicPageViews();

        action.Should().ThrowExactly<TurnierplanException>().WithMessage("Cannot increment page view counter when visibility is not 'Public'.");
    }

    [Fact]
    public void Tournament___IncrementPublicPageViews_When_Tournament_Is_Public___Works_As_Expected()
    {
        var tournament = TestTournament.Create(visibility: Visibility.Public);

        tournament.PublicPageViews.Should().Be(0);
        tournament.IncrementPublicPageViews();
        tournament.PublicPageViews.Should().Be(1);
        tournament.IncrementPublicPageViews();
        tournament.PublicPageViews.Should().Be(2);
    }

    [Fact]
    public void Tournament___IncrementPublicPageViews_When_Limit_Is_Reached___Works_As_Expected()
    {
        var tournament = TestTournament.Create(visibility: Visibility.Public, publicPageViews: int.MaxValue - 1);

        tournament.PublicPageViews.Should().Be(int.MaxValue - 1);
        tournament.IncrementPublicPageViews();
        tournament.PublicPageViews.Should().Be(int.MaxValue);
        tournament.IncrementPublicPageViews();
        tournament.PublicPageViews.Should().Be(int.MaxValue);
    }
}
