using FluentAssertions;
using FluentAssertions.Extensions;
using Turnierplan.Server.Domain;
using Turnierplan.Server.Domain.ApiKeys;
using Xunit;

namespace Turnierplan.Server.Test.Unit.Domain.ApiKeys;

public sealed class ApiKeyTest
{
    [Fact]
    public void SetExpiryDate___With_Valid_And_Invalid_Expiry_Date___Works_As_Expected()
    {
        var currentExpiry = DateTime.UtcNow.AddDays(18);
        var apiKey = new ApiKey(string.Empty, string.Empty, currentExpiry);

        apiKey.ExpiryDate.Should().Be(currentExpiry);

        var expiry2 = currentExpiry.AddDays(7);
        apiKey.SetExpiryDate(expiry2);
        apiKey.ExpiryDate.Should().Be(expiry2);

        var expiry3 = currentExpiry.Subtract(1.Days());
        var action = () => apiKey.SetExpiryDate(expiry3);
        action.Should().ThrowExactly<TurnierplanDomainException>().WithMessage("The new expiry date must be after the currently set expiry date.");
    }
}
