using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
public class UploadController : ApiControllerBase
{
    private readonly IWebHostEnvironment _env;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/api/v1/admin/upload/image")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp" && ext != ".gif")
        {
            return BadRequest("Only image files (.jpg, .jpeg, .png, .webp, .gif) are allowed.");
        }

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsDir = Path.Combine(webRoot, "uploads");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        // Use original clean filename if possible or unique filename
        var safeFileName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");
        var fileName = $"{safeFileName}_{Guid.NewGuid().ToString().Substring(0, 8)}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativeUrl = $"uploads/{fileName}";
        return Ok(new { url = relativeUrl, imageUrl = relativeUrl, fileName = file.FileName });
    }
}
