using IncrementAppVersion.Enums;
using IncrementAppVersion.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
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


        if (Enum.TryParse(input, ignoreCase:true, out ReleaseType requestedReleaseType))
        {
            Console.WriteLine($"Release type: {requestedReleaseType}");
        }
        else
        {
            throw new InvalidCastException("Invalid release type");
        }

        FileVersionService fileVersionService = new FileVersionService(configuration);
        fileVersionService.LoadFileContents();

        var version = fileVersionService.GetVersionFromFile();

        var packageVersionService = new PackageVersionService(version);

        packageVersionService.IncrementVersion(requestedReleaseType);
        var newVersion = packageVersionService.GetVersion();

        fileVersionService.UpdateVersionFromFile(newVersion);

        Console.WriteLine("New Version:" + newVersion);
        Console.ReadKey();
    }
}
