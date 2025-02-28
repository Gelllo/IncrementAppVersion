using IncrementAppVersion.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

class Program
{
    static void Main(string[] args)
    {
        //var version = args[0];

        var input = Console.ReadLine().Trim();

        Console.WriteLine($"Version type: {input}");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Add(new JsonConfigurationSource { Path = "appsettings.json", Optional = false, ReloadOnChange = true})
            .Build();

        //Read file contents
        string filePath = configuration.GetSection("FilePath").Value.ToString();
        string contents = File.ReadAllText(filePath);
        //Parse input into enum type

        if(Enum.TryParse(input, ignoreCase:true, out ReleaseType releaseType))
        {
            Console.WriteLine($"Release type: {releaseType}");
        }
        else
        {
            throw new InvalidCastException("Invalid release type");
        }

        //Based on input type increment version

        string currentReleaseString = "1.0.0.0";

        uint[] releaseParsed = currentReleaseString.Split('.').Select(uint.Parse).ToArray();

        var index = (int)releaseType;

        releaseParsed[index]++;
        while (index < 3)
        {
            releaseParsed[++index] = 0;
        }

        string newVersion = string.Join(".", releaseParsed);

        Console.WriteLine("New version: " + newVersion);
        //Write to file contents

        string newContents = contents.Replace(currentReleaseString, newVersion);

        File.WriteAllText(filePath, newContents);

        Console.ReadKey();
    }
}
