using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Experiences;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
[Route("api/v1/experiences")]
public class ExperiencesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await Mediator.Send(new GetExperiencesQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExperienceCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExperienceCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteExperienceCommand(id));
        return result ? Ok(new { Success = true }) : NotFound();
    }
}
