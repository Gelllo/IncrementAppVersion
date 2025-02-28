using IncrementAppVersion.Enums;

class Program
{
    static void Main(string[] args)
    {
        //var version = args[0];

        var input = Console.ReadLine().Trim();

        Console.WriteLine($"Version type: {input}");

        //Read file contents
        string filePath = "";
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
