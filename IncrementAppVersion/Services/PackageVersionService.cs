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

        public PackageVersionService()
        {
        }

        public PackageVersionService(string version)
        {
            _version = ParseVersion(version);
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Increment version failed: {ex.Message}");
            }

        }

        public uint[] ParseVersion(string version)
        {
            try
            {
                _version = version.Split('.')
                    .Select(uint.Parse)

                    .ToArray();

                if (_version.Count() != 4)
                    throw new FormatException("The version need to contain 4 digits");

                return _version;
            }
            catch (FormatException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetVersion() => string.Join(".", _version);
    }
}
