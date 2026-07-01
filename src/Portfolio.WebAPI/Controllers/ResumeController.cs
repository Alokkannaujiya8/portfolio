using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Resumes;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
public class ResumeController : ApiControllerBase
{
    private readonly IWebHostEnvironment _env;

    public ResumeController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet("/api/v1/public/resume/download")]
    public async Task<IActionResult> DownloadActiveResume()
    {
        var resume = await Mediator.Send(new GetActiveResumeQuery());
        if (resume == null)
        {
            return NotFound("No active resume found.");
        }

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var filePath = Path.Combine(webRoot, resume.FilePath.Replace('/', Path.DirectorySeparatorChar));

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Resume file not found on disk.");
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(bytes, "application/pdf", resume.FileName);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/api/v1/admin/resume")]
    public async Task<IActionResult> GetAllResumes()
    {
        return Ok(await Mediator.Send(new GetResumesQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/api/v1/admin/resume/upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
        {
            return BadRequest("Only PDF files are allowed.");
        }

        using var stream = file.OpenReadStream();
        var result = await Mediator.Send(new UploadResumeCommand(stream, file.FileName));
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/v1/admin/resume")]
    public async Task<IActionResult> Activate([FromBody] ActivateResumeRequest request)
    {
        var result = await Mediator.Send(new ActivateResumeCommand(request.Id));
        return result ? Ok(new { Success = true }) : NotFound();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("/api/v1/admin/resume/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteResumeCommand(id));
        return result ? Ok(new { Success = true }) : NotFound();
    }
}

public record ActivateResumeRequest(Guid Id);
