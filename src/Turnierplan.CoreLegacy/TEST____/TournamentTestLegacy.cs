namespace Turnierplan.Core.Test.Unit.TournamentPlanner;

internal sealed class TournamentTestLegacy
{
    
    [Fact]
    public void Tournament___SetMatchOrder_With_Valid_Input___Works_As_Expected()
    {
        // Arrange
        var tournament = TestTournament.Default;
        tournament._matches.Add(new Match(1, 1, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(2, 2, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(3, 3, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(4, 4, new NullTeamSelector(), new NullTeamSelector(), null!));

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
        var tournament = new Tournament(string.Empty);
        tournament._matches.Add(new Match(1, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(2, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(3, new NullTeamSelector(), new NullTeamSelector(), null!));
        tournament._matches.Add(new Match(4, new NullTeamSelector(), new NullTeamSelector(), null!));

        // Act
        var order = Enumerable.Range(0, matchIds.Length).ToDictionary(i => matchIds[i], i => matchIndices[i]);
        var action = () => tournament.SetMatchOrder(order);

        // Assert
        action.Should().ThrowExactly<TurnierplanException>().WithMessage(expectedExceptionMessage);
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
