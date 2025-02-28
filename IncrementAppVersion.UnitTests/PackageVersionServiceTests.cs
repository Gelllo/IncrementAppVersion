using IncrementAppVersion.Enums;
using IncrementAppVersion.Interfaces;
using IncrementAppVersion.Services;
using Moq;

namespace IncrementAppVersion.UnitTests;

public class PackageVersionServiceTests
{
    private readonly Mock<ILoggingService> _mockLogger = new();

    [Fact]
    public void Constructor_WithoutVersion_InitializesToZeros()
    {
        // Arrange
        // Act
        var service = new PackageVersionService(_mockLogger.Object);

        // Assert
        Assert.Equal("0.0.0.0", service.GetVersion());
    }

    [Fact]
    public void Constructor_WithValidVersion_InitializesCorrectly()
    {
        // Arrange
        // Act
        var service = new PackageVersionService("1.2.3.4", _mockLogger.Object);

        // Assert
        Assert.Equal("1.2.3.4", service.GetVersion());
    }

    [Theory]
    [InlineData("1.2.3.4", ReleaseType.Main1, "2.0.0.0")]
    [InlineData("4.2.3.8", ReleaseType.Main2, "4.3.0.0")]
    [InlineData("2.2.2.2", ReleaseType.Feature, "2.2.3.0")]
    [InlineData("1.1.0.0", ReleaseType.BugFix, "1.1.0.1")]
    public void IncrementVersion_WithValidType_IncrementsCorrectly(string initialVersion, ReleaseType type, string expectedVersion)
    {
        // Arrange
        var service = new PackageVersionService(initialVersion, _mockLogger.Object);

        // Act
        service.IncrementVersion(type);

        // Assert
        Assert.Equal(expectedVersion, service.GetVersion());
    }

    [Theory]
    [InlineData("1.2.3.4", new uint[] { 1, 2, 3, 4 })]
    [InlineData("0.0.0.0", new uint[] { 0, 0, 0, 0 })]
    [InlineData("255.255.255.255", new uint[] { 255, 255, 255, 255 })]
    [InlineData("122.122.0.0", new uint[] { 122, 122, 0, 0 })]
    public void ParseVersion_WithValidVersion_ReturnsParsedArray(string versionString, uint[] expectedArray)
    {
        // Arrange
        var service = new PackageVersionService(_mockLogger.Object);

        // Act
        var result = service.ParseVersion(versionString);

        // Assert
        Assert.Equal(expectedArray, result);
    }

    [Theory]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4.5")]
    [InlineData("Madgex")]
    public void ParseVersion_WithInvalidLength_ThrowsFormatException(string versionString)
    {
        // Arrange
        var service = new PackageVersionService(_mockLogger.Object);

        // Act
        // Assert
        Assert.Throws<FormatException>(() => service.ParseVersion(versionString));
        _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<FormatException>()), Times.Once);
    }

    [Fact]
    public void ParseVersion_WithInvalidFormat_HasCharacter_ThrowsFormatException()
    {
        // Arrange
        var service = new PackageVersionService(_mockLogger.Object);
        var versionString = "1.2.3.a";

        // Act
        // Assert
        Assert.Throws<FormatException>(() => service.ParseVersion(versionString));
        _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<FormatException>()), Times.Once);
    }

    [Fact]
    public void ParseVersion_WithInvalidFormat_HasNegativeNumber_ThrowsOverflowException()
    {
        // Arrange
        var service = new PackageVersionService(_mockLogger.Object);
        var versionString = "1.2.-3.4";
        // Act
        // Assert
        Assert.Throws<OverflowException>(() => service.ParseVersion(versionString));
        _mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<OverflowException>()), Times.Once);
    }

    [Fact]
    public void GetVersion_ReturnsFormattedString()
    {
        // Arrange
        var service = new PackageVersionService("2.3.4.5", _mockLogger.Object);

        // Act
        var result = service.GetVersion();

        // Assert
        Assert.Equal("2.3.4.5", result);
    }
}
