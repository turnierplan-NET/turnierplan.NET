namespace Turnierplan.Dal;

public static class ValidationConstants
{
    public static class ApiKey
    {
        public const int MaxNameLength = 25;
        public const int MaxDescriptionLength = 250;
        public const int MaxSecretLength = 30;
    }

    public static class ApiKeyRequest
    {
        public const int MaxPathLength = 150;
    }

    public static class Document
    {
        public const int MaxNameLength = 30;
        public const int MaxConfigurationLength = 1024;
    }

    public static class Folder
    {
        public const int MaxNameLength = 64;
    }

    public static class Group
    {
        public const int MaxDisplayNameLength = 25;
        public const int MaxNumberOfTeams = 9;
    }

    public static class Image
    {
        public const int MaxNameLength = 100;
        public const int MaxFileTypeLength = 5;
    }

    public static class Match
    {
        public const int MaxTeamSelectorLength = 32;
    }

    public static class Organization
    {
        public const int MaxNameLength = 40;
    }

    public static class Role
    {
        public const int MaxNameLength = 16;
    }

    public static class Team
    {
        public const int MaxNameLength = 60;
    }

    public static class Tournament
    {
        public const int MaxNameLength = 60;
        public const int MaxNumberOfTeams = 32;
        public const int MaxNumberOfGroupPhaseRounds = 4;

        public static class PresentationConfiguration
        {
            public const int MaxCustomHeaderLength = 60;
        }
    }

    public static class User
    {
        public const int MaxNameLength = 100;
        public const int MaxEMailLength = 100;
        public const int MaxPasswordLength = 64;
    }

    public static class Venue
    {
        public const int MaxNameLength = 60;
        public const int MaxDescriptionLength = 1000;
        public const int MaxAddressDetailCount = 5;
        public const int MaxAddressDetailLength = 50;
        public const int MaxExternalLinkCount = 3;
        public const int MaxExternalLinkLength = 120;
    }
}
