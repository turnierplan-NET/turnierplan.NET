using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Core.Test.Unit.PlanningRealm;

public sealed class ApplicationTest
{


    [Fact]
    public void Application()
    {
        
    }
    
    
    // TODO: Implement new tests
#if false
    [Fact]
    public void Application___SetNotes___History_Is_Kept_Correctly()
    {
        var application = new Application(null!, null, 0, string.Empty);

        application.Notes.Should().BeEmpty();
        application.NotesHistory.Should().BeEmpty();

        // notes are set for the first time
        application.SetNotes("Test");

        application.Notes.Should().Be("Test");
        application.NotesHistory.Should().BeEmpty();

        // notes are updated for the first time
        application.SetNotes("Hello");

        application.Notes.Should().Be("Hello");
        application.NotesHistory.Should().BeEquivalentTo(["Test"], opt => opt.WithStrictOrdering());

        // make the 'H' lower case
        application.SetNotes("hello");

        application.Notes.Should().Be("hello");
        application.NotesHistory.Should().BeEquivalentTo(["Test"], opt => opt.WithStrictOrdering());

        // notes are updated to something different
        application.SetNotes("Hello2");

        application.Notes.Should().Be("Hello2");
        application.NotesHistory.Should().BeEquivalentTo(["Test", "hello"], opt => opt.WithStrictOrdering());

        // whitespace is automatically trimmed
        application.SetNotes("  Hello2   ");

        application.Notes.Should().Be("Hello2");
        application.NotesHistory.Should().BeEquivalentTo(["Test", "hello"], opt => opt.WithStrictOrdering());

        // notes are updated to something different
        application.SetNotes("Hello World");

        application.Notes.Should().Be("Hello World");
        application.NotesHistory.Should().BeEquivalentTo(["Test", "hello", "Hello2"], opt => opt.WithStrictOrdering());
    }
#endif
}
