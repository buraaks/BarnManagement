using BarnManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarnManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly ISseBroadcaster _broadcaster;

    public EventsController(ISseBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    [HttpGet]
    public async Task Get(CancellationToken cancellationToken)
    {
        var response = Response;
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Connection", "keep-alive");

        await foreach (var message in _broadcaster.GetEventStream(cancellationToken))
        {
            await response.WriteAsync(message, cancellationToken);
            await response.Body.FlushAsync(cancellationToken);
        }
    }
}
