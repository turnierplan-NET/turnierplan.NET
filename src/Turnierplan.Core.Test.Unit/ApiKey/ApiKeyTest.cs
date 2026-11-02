using FluentAssertions;
using FluentAssertions.Extensions;
using Turnierplan.Core.Exceptions;
using Xunit;

namespace Turnierplan.Core.Test.Unit.ApiKey;

public sealed class ApiKeyTest
{
    [Fact]
    public void SetExpiryDate___With_Valid_And_Invalid_Expiry_Date___Works_As_Expected()
    {
        var currentExpiry = DateTime.UtcNow.AddDays(18);
        var organization = new Organization.Organization("Test");
        var apiKey = new Core.ApiKey.ApiKey(organization, string.Empty, string.Empty, currentExpiry);

        apiKey.ExpiryDate.Should().Be(currentExpiry);

        var expiry2 = currentExpiry.AddDays(7);
        apiKey.SetExpiryDate(expiry2);
        apiKey.ExpiryDate.Should().Be(expiry2);

        var expiry3 = currentExpiry.Subtract(1.Days());
        var action = () => apiKey.SetExpiryDate(expiry3);
        action.Should().ThrowExactly<TurnierplanException>().WithMessage("The new expiry date must be after the currently set expiry date.");
    }
}
