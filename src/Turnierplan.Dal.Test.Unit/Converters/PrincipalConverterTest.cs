using Turnierplan.Core.Exceptions;
using Turnierplan.Core.RoleAssignment;
using Turnierplan.Dal.Converters;

namespace Turnierplan.Dal.Test.Unit.Converters;

public sealed class PrincipalConverterTest
{
    [Theory]
    [InlineData(PrincipalKind.ApiKey, "123", "ApiKey:123")]
    [InlineData(PrincipalKind.User, "2e839c1b-04ea-43c9-9bd1-614bf9586859", "User:2e839c1b-04ea-43c9-9bd1-614bf9586859")]
    public void FormatPrincipal___When_Called___Produces_Expected_Result(PrincipalKind kind, string objectId, string expectedResult)
    {
        var principal = new Principal(kind, objectId);

        var result = PrincipalConverter.FormatPrincipal(principal);

        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("ApiKey:123", PrincipalKind.ApiKey, "123")]
    [InlineData("User:2e839c1b-04ea-43c9-9bd1-614bf9586859", PrincipalKind.User, "2e839c1b-04ea-43c9-9bd1-614bf9586859")]
    public void ParsePrincipal___When_Called_With_Valid_String___Returns_Expected_Result(string representation, PrincipalKind expectedKind, string expectedObjectId)
    {
        var result = PrincipalConverter.ParsePrincipal(representation);

        result.Kind.Should().Be(expectedKind);
        result.ObjectId.Should().Be(expectedObjectId);
    }

    [Theory]
    [InlineData("ApiKey:")]
    [InlineData("ApiKey:123 ")]
    [InlineData(" ApiKey:123")]
    [InlineData("ApiKey:4285231c-cd63-4eb5-adb5-1604d00b2e8e")]
    [InlineData("User:123")]
    [InlineData("User:4285231c-cd63-4eb5-adb5-1604d00b2e8")]
    public void ParsePrincipal___When_Called_With_Invalid_String___Throws_Exception(string representation)
    {
        var action = () => PrincipalConverter.ParsePrincipal(representation);

        action.Should().ThrowExactly<TurnierplanException>().WithMessage("Invalid principal string.");
    }
}
