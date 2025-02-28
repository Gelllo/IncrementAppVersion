using IncrementAppVersion.Interfaces;
using Microsoft.Extensions.Logging;

namespace IncrementAppVersion.Services;

public class LoggingService : ILoggingService
{
    private readonly ILogger<LoggingService> _logger;

    public LoggingService(ILogger<LoggingService> logger)
    {
        _logger = logger;
    }
    public void LogError(string message, Exception ex)
    {
        _logger.LogError(message, ex);
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }
}
