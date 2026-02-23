using Backend.Foundation.Template.Application.Dispatching;
using Backend.Foundation.Template.Application.Features.System.GetServerTime;
using Backend.Foundation.Template.Presentation;
using Backend.Foundation.Template.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Foundation.Template.Controllers;

[ApiController]
[Route("api/system")]
[Authorize]
public sealed class SystemController : ControllerBase
{
    private readonly IRequestDispatcher _dispatcher;

    public SystemController(IRequestDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("time")]
    [RequirePermission("system.time.read")]
    public async Task<IActionResult> GetServerTime(CancellationToken ct)
    {
        var result = await _dispatcher.Send(new GetServerTimeQuery(), ct);
        return this.ToActionResult(result, "Server time fetched.");
    }
}
