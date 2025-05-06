using Fantasy.Backend.Helpers.Extensions;
using System.Text.RegularExpressions;
using System.Threading;

namespace Fantasy.Backend.Helpers;

public interface IFileStorage
{
    Task<string> SaveFileAsync(byte[] content, string extention, string containerName);

    Task RemoveFileAsync(string path, string containerName);
}

public class FileStorage : IFileStorage
{
    public FileStorage()
    {
    }

    public async Task RemoveFileAsync(string path, string containerName)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public async Task<string> SaveFileAsync(byte[] content, string extension, string containerName)
    {
        var fileName = $"{Guid.NewGuid()}.{extension}";
        string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), containerName);
        Directory.CreateDirectory(pathToSave);

        fileName = RemoveSpecialCharacters(fileName);
        fileName = fileName.ReplaceWhitespace("-");
        string fullPath = Path.Combine(pathToSave, fileName);
        string dbPath = Path.Combine(containerName, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await stream.WriteAsync(content, 0, content.Length);
        return dbPath.Replace("\\", "/");
    }

    private static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
    }
}