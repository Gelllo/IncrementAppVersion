using IncrementAppVersion.Enums;
using IncrementAppVersion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncrementAppVersion.Services
{
    public class PackageVersionService : IPackageVersionService
    {
        private uint[] _version = [0, 0, 0, 0];
        private readonly ILoggingService _logger;

        public PackageVersionService(ILoggingService logger)
        {
            _logger = logger;
        }

        public PackageVersionService(string version, ILoggingService logger)
        {
            _version = ParseVersion(version);
            _logger = logger;
        }

        public void IncrementVersion(ReleaseType type)
        {
            try
            {
                var index = (int)type;
                _version[index]++;

                while (index < _version.Length - 1)
                {
                    _version[++index] = 0;
                }

                _logger.LogInformation("Version was successfully incremented");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error incrementing the version", ex);
                throw;
            }
        }

        public uint[] ParseVersion(string version)
        {
            try
            {
                _version = version.Split('.')
                    .Select(uint.Parse)
                    .ToArray();

                if (_version.Count() != 4 )
                    throw new FormatException("The version need to contain 4 digits");

                return _version;
            }
            catch (OverflowException ex)
            {
                _logger.LogError("The version must be composed of positive numbers", ex);
                throw;
            }
            catch (FormatException ex)
            {
                _logger.LogError("Incorrect version format", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing the version", ex);
                throw;
            }
        }

        public string GetVersion() => string.Join(".", _version);
    }
}
