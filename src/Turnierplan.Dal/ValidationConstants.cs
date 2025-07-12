namespace Turnierplan.Dal;

public static class ValidationConstants
{
    public static class Group
    {
        public const int MaxNumberOfTeams = 9;
    }

    public static class Tournament
    {
        public const int MaxNumberOfTeams = 32;
        public const int MaxNumberOfGroupPhaseRounds = 4;
    }
}
