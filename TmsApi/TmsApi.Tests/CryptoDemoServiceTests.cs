using TmsApi.Infrastructure.Services;
using Xunit;

namespace TmsApi.Tests;

public class CryptoDemoServiceTests
{
    [Fact]
    public void HashUserPassword_GeneratesUniqueSalts_ForSamePassword()
    {
        // Arrange
        var service = new CryptoDemoService();
        var rawPassword = "Password123!";

        // Act
        var hash1 = service.HashUserPassword(rawPassword);
        var hash2 = service.HashUserPassword(rawPassword);

        // Assert - Salt uniqueness ensures hashes are never equal
        Assert.NotEqual(hash1, hash2);

        // Both verify to true against the same plain text
        Assert.True(service.VerifyUserPassword(rawPassword, hash1));
        Assert.True(service.VerifyUserPassword(rawPassword, hash2));

        // Wrong password fails verification
        Assert.False(service.VerifyUserPassword("WrongPassword!", hash1));
    }
}
