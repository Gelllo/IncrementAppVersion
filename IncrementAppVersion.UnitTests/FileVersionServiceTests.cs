using IncrementAppVersion.Interfaces;
using IncrementAppVersion.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace IncrementAppVersion.UnitTests;

public class FileVersionServiceTests
{
    private readonly Mock<ILoggingService> _logger = new();
    private readonly Mock<IConfiguration> _mockConfig = new();
    private readonly string testFilePath = Path.GetTempFileName();

    public FileVersionServiceTests()
    {
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns(testFilePath);
        _mockConfig.Setup(c => c.GetSection("FilePath"))
            .Returns(mockSection.Object);
    }

    [Fact]
    public void ReadFileVersion_ValidFile_ReturnsFileContent()
    {
        // Arrange

        var expectedContent = "version=\"1.0.0.0\"";

        File.WriteAllText(testFilePath, expectedContent);

        var service = new FileVersionService(_mockConfig.Object, _logger.Object);

        try
        {
            // Act
            service.LoadFileContents();

            // Assert
            Assert.Equal(expectedContent, service.GetFileContents());
        }
        finally
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }

    [Fact]
    public void UpdateVersionFromFile_ValidContent_WritesToFile()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        var testFilePath = Path.GetTempFileName();
        var existingContent = "version=\"1.0.0.0\"";
        var contentToWrite = "version=\"2.0.0.0\"";

        var service = new FileVersionService(_mockConfig.Object, _logger.Object);

        File.WriteAllText(testFilePath, existingContent);

        try
        {
            // Act
            service.LoadFileContents();
            service.UpdateVersionFromFile(contentToWrite);

            // Assert

            Assert.NotEqual(contentToWrite, service.GetFileContents());
        }
        finally
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }

    [Fact]
    public void GetVersionFromFile_ValidPattern_ReturnsVersionNumber()
    {
        // Arrange
        var expectedVersion = "2.0.0.1";
        var existingContent = "Madgex Madgex version=\"2.0.0.1\" Madgex Madgex";

        File.WriteAllText(testFilePath, existingContent);

        var service = new FileVersionService(_mockConfig.Object, _logger.Object);

        try
        {
            // Act
            service.LoadFileContents();
            var result = service.GetVersionFromFile();

            // Assert
            Assert.Equal(expectedVersion, result);
        }
        finally
        {
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }
    }
}

