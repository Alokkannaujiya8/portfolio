using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.SEO;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
public class SEOController : ApiControllerBase
{
    [HttpGet("/api/v1/seo/{pageName}")]
    public async Task<IActionResult> GetByPage(string pageName)
    {
        var seo = await Mediator.Send(new GetSEOQuery(pageName));
        return seo == null ? NotFound() : Ok(seo);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/api/v1/admin/seo")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await Mediator.Send(new GetAllSEOQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/v1/admin/seo")]
    public async Task<IActionResult> Update([FromBody] UpdateSEOCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
