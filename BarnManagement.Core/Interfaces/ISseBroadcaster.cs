namespace BarnManagement.Core.Interfaces;

public interface ISseBroadcaster
{
    Task BroadcastUpdateAsync(string eventName, string data = "", CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetEventStream(CancellationToken cancellationToken);
}
