using Fantasy.Shared.DTOs.Generic;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPut("uploadfile")]
    public async Task<IActionResult> UploadFile([FromBody] FileDTO fileDto)
    {
        using var stream = new MemoryStream();
        string fileName = await _fileService.SaveFileAsync(fileDto.Container, Path.GetExtension(fileDto.Path), stream);
        return Ok(new { FileName = fileName });
    }

    [HttpGet("downloadfile")]
    public IActionResult DownloadFile([FromQuery] FileDTO fileDto)
    {
        var fileStream = _fileService.GetFile(fileDto.Container, fileDto.Path);
        if (fileStream == null)
            return NotFound("El archivo no existe.");

        return File(fileStream, "application/octet-stream", fileDto.Path);
    }

    [HttpDelete("deletefile")]
    public IActionResult DeleteFile([FromBody] FileDTO fileDto)
    {
        bool result = _fileService.DeleteFile(fileDto.Container, fileDto.Path);
        if (!result)
            return NotFound("El archivo no existe o no pudo ser eliminado.");

        return Ok("Archivo eliminado exitosamente.");
    }

    [HttpPut("uploaduserphoto")]
    public async Task<IActionResult> UploadUserPhoto([FromBody] string filePath)
    {
        string fileName = await _fileService.SaveUserPhotoAsync(filePath);
        return Ok(new { FileName = fileName });
    }

    [HttpGet("downloaduserphotoUrl")]
    public IActionResult DownloadUserPhotoUrl([FromQuery] string filePath)
    {
        string? photoUrl = _fileService.GetUserPhotoUrl(filePath);
        if (photoUrl == null)
            return NotFound("La foto de usuario no existe.");

        return Ok(new { Url = photoUrl });
    }

    [HttpGet("downloaduserphoto/{fileName}")]
    public IActionResult DownloadUserPhoto(string fileName)
    {
        var fileStream = _fileService.GetUserPhoto(fileName);
        if (fileStream == null)
            return NotFound();
        return File(fileStream, "image/jpeg");
    }

    [HttpGet("downloaduserphotoData")]
    public IActionResult DownloadUserPhotoData(string fileName)
    {
        var fileData = _fileService.GetUserPhotoData(fileName);
        if (fileData == null)
            return NotFound();
        return Ok(fileData);
    }

    [HttpDelete("deleteuserphoto")]
    public IActionResult DeleteUserPhoto([FromBody] string filePath)
    {
        bool result = _fileService.DeleteUserPhoto(filePath);
        if (!result)
            return NotFound("La foto de usuario no existe o no pudo ser eliminada.");

        return Ok("Foto de usuario eliminada exitosamente.");
    }
}