using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Provides methods for file operations.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Loads a file stream from the specified local file path.
    /// </summary>
    /// <param name="localFilePath">The local file path.</param>
    /// <returns>A stream of the file, or null if the file does not exist.</returns>
    Stream? LoadFileStream(string localFilePath);

    /// <summary>
    /// Saves a file asynchronously.
    /// </summary>
    /// <param name="folder">The folder where the file will be saved.</param>
    /// <param name="extension">The file extension.</param>
    /// <param name="fileStream">The stream of the file to save.</param>
    /// <returns>The new file name.</returns>
    /// <exception cref="Exception">Thrown when the file could not be saved.</exception>
    Task<string> SaveFileAsync(string folder, string extension, Stream fileStream);

    /// <summary>
    /// Gets a file stream with the given file name.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream of the file, or null if the file does not exist.</returns>
    Stream? GetFile(string fileName);

    /// <summary>
    /// Gets a file stream from the specified folder and file name.
    /// </summary>
    /// <param name="folder">The folder containing the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream of the file, or null if the file does not exist.</returns>
    Stream? GetFile(string folder, string fileName);

    /// <summary>
    /// Gets the URL of a file in the specified folder with the given file name.
    /// </summary>
    /// <param name="folder">The folder containing the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The URL of the file, or null if the file does not exist.</returns>
    string? GetFileUrl(string folder, string fileName);

    /// <summary>
    /// Deletes a file from the specified folder with the given file name.
    /// </summary>
    /// <param name="folder">The folder containing the file.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>True if the file was deleted, otherwise false.</returns>
    bool DeleteFile(string folder, string fileName);

    /// <summary>
    /// Saves a user photo asynchronously.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The new file name.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the file does not exist.</exception>
    /// <exception cref="Exception">Thrown when the file could not be saved.</exception>
    Task<string> SaveUserPhotoAsync(string fileName);

    /// <summary>
    /// Gets a user photo stream with the given file name.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>A stream of the photo, or null if the photo does not exist.</returns>
    FileStream? GetUserPhoto(string fileName);

    /// <summary>
    /// Gets the URL of a user photo with the given file name.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The URL of the photo, or null if the photo does not exist.</returns>
    string? GetUserPhotoUrl(string fileName);

    string? GetUserPhotoData(string fileName);

    /// <summary>
    /// Deletes a user photo with the given file name.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>True if the photo was deleted, otherwise false.</returns>
    bool DeleteUserPhoto(string fileName);
}