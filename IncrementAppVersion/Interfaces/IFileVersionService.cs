using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IncrementAppVersion.Interfaces
{
    public interface IFileVersionService
    {
        void LoadFileContents();

        void UpdateVersionFromFile(string version);

        string GetVersionFromFile();
    }
}
