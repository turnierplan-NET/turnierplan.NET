using System.Text;

namespace Turnierplan.Core.Test.Regression;

internal static class SubjectSerializer
{
    public static void AppendSubject<TSubject>(this StringBuilder builder, TSubject subject)
    {
        switch (subject)
        {
            case Tournament.Tournament tournament:
                AppendTournament(builder, tournament);
                break;
            default:
                throw new InvalidOperationException($"Subject type '{typeof(TSubject).FullName}' currently not supported for serialization.");
        }
    }

    private static void AppendTournament(StringBuilder builder, Tournament.Tournament tournament)
    {
        builder.Append(tournament.Name);
        builder.Append(';');
        builder.Append(tournament.Visibility);
        builder.Append(';');
        builder.Append(tournament.IsPublic);
        builder.Append(';');
        builder.Append(tournament.PublicPageViews);
        builder.Append(';');
        builder.Append(tournament.Teams.Count);
        builder.Append(';');
        builder.Append(tournament.Groups.Count);
        builder.Append(';');
        builder.Append(tournament.Matches.Count);
        builder.AppendLine();
    }
}
