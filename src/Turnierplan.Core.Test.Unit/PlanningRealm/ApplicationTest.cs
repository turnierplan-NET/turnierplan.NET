using Turnierplan.Core.PlanningRealm;

namespace Turnierplan.Core.Test.Unit.PlanningRealm;

public sealed class ApplicationTest
{
    private readonly Application _application = new(null!, null, 0, string.Empty);

    [Fact]
    public void Application___When_Notes_Are_Set___Change_Log_Is_Created()
    {
        _application.ChangeLog.Should().BeEmpty();

        SetPropertyAndAssertChangeLogCreated(value => _application.Notes = value!, isNullable: false, ApplicationChangeLogType.NotesChanged);

        _application.ChangeLog.Should().NotBeEmpty();
    }

    [Fact]
    public void Application___When_Contact_Is_Set___Change_Log_Is_Created()
    {
        _application.ChangeLog.Should().BeEmpty();

        SetPropertyAndAssertChangeLogCreated(value => _application.Contact = value!, isNullable: false, ApplicationChangeLogType.ContactChanged);

        _application.ChangeLog.Should().NotBeEmpty();
    }

    [Fact]
    public void Application___When_ContactEmail_Is_Set___Change_Log_Is_Created()
    {
        _application.ChangeLog.Should().BeEmpty();

        SetPropertyAndAssertChangeLogCreated(value => _application.ContactEmail = value, isNullable: false, ApplicationChangeLogType.ContactEmailChanged);

        _application.ChangeLog.Should().NotBeEmpty();
    }

    [Fact]
    public void Application___When_ContactTelephone_Is_Set___Change_Log_Is_Created()
    {
        _application.ChangeLog.Should().BeEmpty();

        SetPropertyAndAssertChangeLogCreated(value => _application.ContactTelephone = value, isNullable: false, ApplicationChangeLogType.ContactTelephoneChanged);

        _application.ChangeLog.Should().NotBeEmpty();
    }

    [Fact]
    public void Application___When_Comment_Is_Set___Change_Log_Is_Created()
    {
        _application.ChangeLog.Should().BeEmpty();

        SetPropertyAndAssertChangeLogCreated(value => _application.Comment = value, isNullable: false, ApplicationChangeLogType.CommentChanged);

        _application.ChangeLog.Should().NotBeEmpty();
    }

    private void SetPropertyAndAssertChangeLogCreated(Action<string?> set, bool isNullable, ApplicationChangeLogType expectedChangeLogType)
    {
        set("Test");

        _application.ChangeLog.Should().HaveCount(1);
        var entry = _application.ChangeLog[^1];
        entry.Type.Should().Be(expectedChangeLogType);
        entry.Properties.Should().HaveCount(2);
        entry.Properties.Single(x => x.Type is ApplicationChangeLogProperty.PreviousValue).Value.Should().Be(string.Empty);
        entry.Properties.Single(x => x.Type is ApplicationChangeLogProperty.NewValue).Value.Should().Be("Test");

        set("Test   ");
        _application.ChangeLog.Should().HaveCount(1);

        set("Hello");

        _application.ChangeLog.Should().HaveCount(2);
        entry = _application.ChangeLog[^1];
        entry.Type.Should().Be(expectedChangeLogType);
        entry.Properties.Should().HaveCount(2);
        entry.Properties.Single(x => x.Type is ApplicationChangeLogProperty.PreviousValue).Value.Should().Be("Test");
        entry.Properties.Single(x => x.Type is ApplicationChangeLogProperty.NewValue).Value.Should().Be("Hello");

        if (isNullable)
        {
            set(null);

            _application.ChangeLog.Should().HaveCount(3);
            entry = _application.ChangeLog[^1];
            entry.Type.Should().Be(expectedChangeLogType);
            entry.Properties.Should().HaveCount(2);
            entry.Properties.Single(x => x.Type is ApplicationChangeLogProperty.PreviousValue).Value.Should().Be("Hello");
            entry.Properties.Single(x => x.Type is ApplicationChangeLogProperty.NewValue).Value.Should().Be(string.Empty);
        }
    }
}
