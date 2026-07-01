using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Features.Home;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
public class HomeController : ApiControllerBase
{
    [HttpGet("/api/v1/home/hero")]
    public async Task<IActionResult> GetHero()
    {
        var hero = await Mediator.Send(new GetHeroQuery());
        return hero == null ? NotFound() : Ok(hero);
    }

    [HttpGet("/api/v1/home/about")]
    public async Task<IActionResult> GetAbout()
    {
        var about = await Mediator.Send(new GetAboutQuery());
        return about == null ? NotFound() : Ok(about);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/v1/admin/home/hero")]
    public async Task<IActionResult> UpdateHero([FromBody] UpdateHeroCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("/api/v1/admin/home/about")]
    public async Task<IActionResult> UpdateAbout([FromBody] UpdateAboutCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
