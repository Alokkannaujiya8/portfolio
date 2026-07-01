using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Blogs;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
[Route("api/v1/blogs")]
public class BlogsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyPublished = false)
    {
        var isUserAdmin = User.Identity?.IsAuthenticated == true && User.IsInRole("Admin");
        var actualOnlyPublished = !isUserAdmin || onlyPublished;
        return Ok(await Mediator.Send(new GetBlogsQuery(actualOnlyPublished)));
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var blog = await Mediator.Send(new GetBlogBySlugQuery(slug));
        if (blog == null) return NotFound();
        
        if (!blog.IsPublished && !(User.Identity?.IsAuthenticated == true && User.IsInRole("Admin")))
        {
            return NotFound();
        }

        return Ok(blog);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBlogCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlogCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch.");
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteBlogCommand(id));
        return result ? Ok(new { Success = true }) : NotFound();
    }
}
