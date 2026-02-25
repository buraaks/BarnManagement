using System.Threading.Channels;
using BarnManagement.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace BarnManagement.Business.Services;

public class SseBroadcaster : ISseBroadcaster
{
    private readonly Channel<string> _channel;
    private readonly ILogger<SseBroadcaster> _logger;

    public SseBroadcaster(ILogger<SseBroadcaster> logger)
    {
        _logger = logger;
        // Unbounded channel for simplicity, though in a high-load scenario we'd use bounded.
        _channel = Channel.CreateUnbounded<string>();
    }

    public async Task BroadcastUpdateAsync(string eventName, string data = "", CancellationToken cancellationToken = default)
    {
        var message = $"event: {eventName}\ndata: {data}\n\n";
        await _channel.Writer.WriteAsync(message, cancellationToken);
        _logger.LogDebug("Broadcasted event: {EventName}", eventName);
    }

    public IAsyncEnumerable<string> GetEventStream(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
