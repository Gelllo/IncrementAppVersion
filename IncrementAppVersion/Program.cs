using IncrementAppVersion.Enums;
using IncrementAppVersion.Interfaces;
using IncrementAppVersion.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        var input = args[0];

        var configuration = GetRequiredConfiguration();

        ConfigureLogging(configuration.GetSection("LoggingFile").Value.ToString());        

        var serviceProvider = GetRequiredServices();

        var logger = serviceProvider.GetService<ILoggingService>();

        IncrementVersionNumber(input, configuration, logger);    
    }

    private static IConfiguration GetRequiredConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true })
            .Build();
    }

    private static ServiceProvider GetRequiredServices()
    {
        return new ServiceCollection()
             .AddLogging(loggingBuilder =>
             {
                 loggingBuilder.ClearProviders();
                 loggingBuilder.AddSerilog();
             })
             .AddSingleton<ILoggingService, LoggingService>()
             .BuildServiceProvider();
    }

    private static void ConfigureLogging(string logFilePath)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(logFilePath)
            .CreateLogger();
    }

    private static void IncrementVersionNumber(string input, IConfiguration configuration, ILoggingService logger)
    {
        if (Enum.TryParse(input, ignoreCase: true, out ReleaseType requestedReleaseType))
        {
            Log.Information($"Requested Release Type matches: {requestedReleaseType}");
        }
        else
        {
            var ex = new InvalidCastException("Requested release could not be matched");
            Log.Error("Invalid release type", ex);
        }

        FileVersionService fileVersionService = new FileVersionService(configuration, logger);
        fileVersionService.LoadFileContents();

        var version = fileVersionService.GetVersionFromFile();

        var packageVersionService = new PackageVersionService(version, logger);

        packageVersionService.IncrementVersion(requestedReleaseType);

        var newVersion = packageVersionService.GetVersion();

        fileVersionService.UpdateVersionFromFile(newVersion);
    }
}
