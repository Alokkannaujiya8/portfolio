using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Educations;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
[Route("api/v1/educations")]
public class EducationsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await Mediator.Send(new GetEducationsQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEducationCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEducationCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteEducationCommand(id));
        return result ? Ok(new { Success = true }) : NotFound();
    }
}
