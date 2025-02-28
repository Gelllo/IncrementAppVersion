namespace IncrementAppVersion.Interfaces;

public interface IFileVersionService
{
    void LoadFileContents();

    void UpdateVersionFromFile(string version);

    string GetVersionFromFile();

    string GetFileContents();
}

