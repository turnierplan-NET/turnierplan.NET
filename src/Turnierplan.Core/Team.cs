namespace Turnierplan.Core;

public sealed class Team
{
    internal Team(string name, bool outOfCompetition)
    {
        Name = name;
        OutOfCompetition = outOfCompetition;
        Statistics = new TeamStatistics();
    }

    public string Name { get; set; }

    public bool OutOfCompetition { get; set; }

    public TeamStatistics Statistics { get; }
}
