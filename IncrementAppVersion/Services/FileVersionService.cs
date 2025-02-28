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
        private readonly ILoggingService _logger;
        private string fileContents = string.Empty;

        public FileVersionService(IConfiguration configuration, ILoggingService logger)
        {
            FileVersionPath = configuration.GetSection("FilePath").Value.ToString();
            _logger = logger;
        }

        public void LoadFileContents()
        {
            try
            {
                fileContents = File.ReadAllText(FileVersionPath);
                _logger.LogInformation("File contents loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading the file contents", ex);
                throw;
            }
        }

        public void UpdateVersionFromFile(string version)
        {
            try
            {
                var updatedFileContents = Regex.Replace(fileContents, VersionLookup, version);

                File.WriteAllText(FileVersionPath, updatedFileContents);

                _logger.LogInformation("Version updated successfully: NEW VERSION:" + version);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating the version", ex);
                throw;
            }
        }

        public string GetVersionFromFile()
        {
            Match match = Regex.Match(fileContents, VersionLookup);
            if (match.Success)
            {
                _logger.LogInformation("Version found successfully");
                return match.Groups[1].Value;
            }
            else
            {
                var ex = new InvalidOperationException();
                _logger.LogError("Could not find the version in the specified file", ex);
                throw ex;
            }
        }

        public string GetFileContents() => fileContents;
    }
}
