using IncrementAppVersion.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IncrementAppVersion.Services
{
    public class FileVersionService : IFileVersionService
    {
        private readonly string? VersionLookup = "(\\d+\\.\\d+\\.\\d+\\.\\d+)";
        private readonly string? FileVersionPath;
        private string fileContents = string.Empty;

        public FileVersionService(IConfiguration configuration)
        {
            FileVersionPath = configuration.GetSection("FilePath").Value.ToString();
        }

        public void LoadFileContents()
        {
            try
            {
                fileContents = File.ReadAllText(FileVersionPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading the file: {ex.Message}", ex);
            }
        }

        public void UpdateVersionFromFile(string version)
        {
            try
            {
                var updatedFileContents = Regex.Replace(fileContents, VersionLookup, version);

                if (updatedFileContents != fileContents)
                {
                    File.WriteAllText(FileVersionPath, updatedFileContents);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating version from file: {ex.Message}", ex);
            }
        }

        public string GetVersionFromFile()
        {
            Match match = Regex.Match(fileContents, VersionLookup);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new Exception("Version not found in the file");
            }
        }
    }
}
