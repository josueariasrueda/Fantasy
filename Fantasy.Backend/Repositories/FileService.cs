using System;
using System.IO;
using System.Threading.Tasks;
using Fantasy.Backend.Helpers;
using Fantasy.Backend.MultiTenant;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.X509;

public interface IFileService
{
    Stream? LoadFileStream(string localFilePath);

    Task<string> SaveFileAsync(string folder, string extension, Stream fileStream);

    Stream? GetFile(string fileName);

    Stream? GetFile(string folder, string fileName);

    string? GetFileUrl(string folder, string fileName);

    bool DeleteFile(string folder, string fileName);

    Task<string> SaveUserPhotoAsync(string fileName);

    FileStream? GetUserPhoto(string fileName);

    string? GetUserPhotoUrl(string fileName);

    string? GetUserPhotoData(string fileName);

    bool DeleteUserPhoto(string fileName);
}

/// <summary>
/// Provides file-related services such as saving, loading, and deleting files.
/// </summary>
public class FileService : IFileService
{
    private readonly string _basePath;
    private readonly ICurrentTenant _currentTenant;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileService"/> class.
    /// </summary>
    /// <param name="currentTenant">The current tenant.</param>
    public FileService(ICurrentTenant currentTenant)
    {
        _basePath = Path.Combine("wwwroot", "uploads");
        _currentTenant = currentTenant;
        Directory.CreateDirectory(_basePath);
    }

    // En los campos donde se almacene el nombre de algun archivo debe ir únicamente /filename.extension
    // Siempre debe pasarse el nombre del folder donde se debe de buscar el archivo
    // El tenent se agrega automaticamente
    // Los usuarios estan en el tenent root
    // El path de un archivo es la combinacion de tenant/folder/filename.extension

    private string GetBaseAndTenantPath() => Path.Combine(_basePath, _currentTenant.GetCurrentTenant().StoragePath);

    /// <summary>
    /// Loads a file stream from the specified local file path.
    /// </summary>
    /// <param name="localFilePath">The local file path.</param>
    /// <returns>A stream of the file if it exists, otherwise null.</returns>
    public Stream? LoadFileStream(string localFilePath)
    {
        if (File.Exists(localFilePath))
        {
            return new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
        }
        return null;
    }

    /// <summary>
    /// Saves a file asynchronously.
    /// </summary>
    /// <param name="folder">The folder where the file will be saved.</param>
    /// <param name="extension">The file extension.</param>
    /// <param name="fileStream">The stream of the file to save.</param>
    /// <returns>The new file name.</returns>
    /// <exception cref="Exception">Thrown when the file could not be saved.</exception>
    public async Task<string> SaveFileAsync(string folder, string extension, Stream fileStream)
    {
        string tenantPath = GetBaseAndTenantPath();
        string folderPath = Path.Combine(tenantPath, folder);
        Directory.CreateDirectory(folderPath);

        string newFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(folderPath, newFileName);

        using FileStream outputFileStream = new(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputFileStream);
        if (outputFileStream.Length == 0)
        {
            throw new Exception("The file could not be saved.");
        }

        return newFileName;
    }

    /// <summary>
    /// Retrieves a file as a stream.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream of the file if it exists, otherwise null.</returns>
    public Stream? GetFile(string fileName)
    {
        return File.Exists(fileName) ? new FileStream(fileName, FileMode.Open, FileAccess.Read) : null;
    }

    /// <summary>
    /// Retrieves a file as a stream.
    /// </summary>
    /// <param name="folder">The folder where the file is located.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream of the file if it exists, otherwise null.</returns>
    public Stream? GetFile(string folder, string fileName)
    {
        string filePath = Path.Combine(GetBaseAndTenantPath(), folder, fileName);
        return File.Exists(filePath) ? new FileStream(filePath, FileMode.Open, FileAccess.Read) : null;
    }

    /// <summary>
    /// Gets the URL of a file.
    /// </summary>
    /// <param name="folder">The folder where the file is located.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The URL of the file if it exists, otherwise null.</returns>
    public string? GetFileUrl(string folder, string fileName)
    {
        string filePath = Path.Combine(GetBaseAndTenantPath(), folder, fileName);
        return File.Exists(filePath) ? $"/{GetBaseAndTenantPath()}/{folder}/{fileName}" : null;
    }

    /// <summary>
    /// Deletes a file from the specified folder.
    /// </summary>
    /// <param name="folder">The folder where the file is located.</param>
    /// <param name="fileName">The name of the file to delete.</param>
    /// <returns>True if the file was successfully deleted, otherwise false.</returns>
    public bool DeleteFile(string folder, string fileName)
    {
        string filePath = Path.Combine(GetBaseAndTenantPath(), folder, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Saves a user photo asynchronously.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The new file name.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="Exception">Thrown when the file could not be saved.</exception>
    public async Task<string> SaveUserPhotoAsync(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string newFileName = $"{Guid.NewGuid()}{extension}";
        string userDirectory = Path.Combine(_basePath, "root/user");
        string filePath = Path.Combine(userDirectory, newFileName);
        Directory.CreateDirectory(userDirectory);

        Stream? fileStream = GetFile(fileName);
        if (fileStream == null)
        {
            throw new FileNotFoundException("The file does not exist.");
        }

        using FileStream outputFileStream = new(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputFileStream);
        if (outputFileStream.Length == 0)
        {
            throw new Exception("The file could not be saved.");
        }

        return newFileName;
    }

    /// <summary>
    /// Retrieves a user photo as a stream.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream of the user photo if it exists, otherwise null.</returns>
    public FileStream? GetUserPhoto(string fileName)
    {
        string filePath = Path.Combine(_basePath, "root", "user", fileName);
        return System.IO.File.Exists(filePath) ? System.IO.File.OpenRead(filePath) : null;
    }

    /// <summary>
    /// Gets the URL of a user photo.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The URL of the user photo if it exists, otherwise null.</returns>
    public string? GetUserPhotoUrl(string fileName)
    {
        string filePath = Path.Combine(_basePath, "root", "user", fileName);
        //Console.WriteLine($"filePath: {filePath}");
        return File.Exists(filePath) ? $"{filePath.Replace("\\", "/")}" : null;
    }

    /// <summary>
    /// Gets the URL of a user photo.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The URL of the user photo if it exists, otherwise null.</returns>
    public string? GetUserPhotoData(string fileName)
    {
        string filePath = Path.Combine(_basePath, "root", "user", fileName).Replace("\\", "/");
        var encodedPhotoName = Uri.EscapeDataString(filePath);
        byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
        return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
    }

    /// <summary>
    /// Deletes a user photo from the specified file path.
    /// </summary>
    /// <param name="fileName">The name of the file to delete.</param>
    /// <returns>True if the file was successfully deleted, otherwise false.</returns>
    public bool DeleteUserPhoto(string fileName)
    {
        string filePath = Path.Combine(_basePath, "/root/user", fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }
}