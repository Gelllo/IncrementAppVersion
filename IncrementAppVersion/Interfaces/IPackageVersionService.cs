using IncrementAppVersion.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncrementAppVersion.Interfaces
{
    public interface IPackageVersionService
    {
        public void IncrementVersion(ReleaseType type);
        public uint[] ParseVersion(string version);
        public string GetVersion();
    }
}
