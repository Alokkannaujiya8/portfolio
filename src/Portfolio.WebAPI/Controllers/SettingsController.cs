using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Settings;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
public class SettingsController : ApiControllerBase
{
    [HttpGet("/api/v1/settings")]
    public async Task<IActionResult> Get()
    {
        return Ok(await Mediator.Send(new GetSettingsQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/v1/admin/settings")]
    public async Task<IActionResult> Update([FromBody] Dictionary<string, string> settings)
    {
        return Ok(await Mediator.Send(new UpdateSettingsCommand(settings)));
    }
}
