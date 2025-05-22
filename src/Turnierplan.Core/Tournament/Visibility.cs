namespace Turnierplan.Core.Tournament;

public enum Visibility
{
    // Note: Don't change enum values (DB serialization)

    /// <summary>
    /// The entity is only accessible by its owner or any authorized user through the authenticated API.
    /// </summary>
    Private = 1,

    /// <summary>
    /// The entity is accessible via the methods described in <see cref="Private"/> or through the public, unauthenticated API.
    /// </summary>
    Public = 2
}
