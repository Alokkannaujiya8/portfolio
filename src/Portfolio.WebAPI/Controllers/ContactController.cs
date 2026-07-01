using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.ContactMessages;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
public class ContactController : ApiControllerBase
{
    [HttpPost("/api/v1/contact")]
    public async Task<IActionResult> Submit([FromBody] SubmitContactMessageCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/api/v1/admin/contact/messages")]
    public async Task<IActionResult> GetAllMessages()
    {
        return Ok(await Mediator.Send(new GetContactMessagesQuery()));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/v1/admin/contact/messages/{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await Mediator.Send(new MarkMessageAsReadCommand(id));
        return result ? Ok(new { Success = true }) : NotFound();
    }
}
