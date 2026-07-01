using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Skills;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
[Route("api/v1/skills")]
public class SkillsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await Mediator.Send(new GetSkillsQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSkillCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSkillCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteSkillCommand(id));
        return result ? Ok(new { Success = true }) : NotFound();
    }
}
