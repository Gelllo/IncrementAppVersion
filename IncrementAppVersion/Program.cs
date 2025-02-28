using IncrementAppVersion.Enums;
using IncrementAppVersion.Interfaces;
using IncrementAppVersion.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

class Program
{
    static void Main(string[] args)
    {
        //var version = args[0];

        var input = Console.ReadLine().Trim();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true})
        .Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Set the minimum log level
            .WriteTo.Console() // Write to console
            .WriteTo.File(configuration.GetSection("LoggingFile").Value.ToString()) // Write to file
            .CreateLogger();

        Log.Information("Starting Increment App Version");

        var serviceProvider = new ServiceCollection()
             .AddLogging(loggingBuilder =>
             {
                 loggingBuilder.ClearProviders();
                 loggingBuilder.AddSerilog();
             })
             .AddSingleton<ILoggingService, LoggingService>()
             .BuildServiceProvider();

        var logger = serviceProvider.GetService<ILoggingService>();

        if (Enum.TryParse(input, ignoreCase:true, out ReleaseType requestedReleaseType))
        {
            Console.WriteLine($"Release type: {requestedReleaseType}");
        }
        else
        {
            var ex = new InvalidCastException("Invalid release type");
            Log.Error("Invalid release type", ex);
        }

        FileVersionService fileVersionService = new FileVersionService(configuration, logger);
        fileVersionService.LoadFileContents();

        var version = fileVersionService.GetVersionFromFile();

        var packageVersionService = new PackageVersionService(version, logger);

        packageVersionService.IncrementVersion(requestedReleaseType);
        var newVersion = packageVersionService.GetVersion();

        fileVersionService.UpdateVersionFromFile(newVersion);

        Console.ReadKey();
    }
}
