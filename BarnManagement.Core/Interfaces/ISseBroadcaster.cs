namespace BarnManagement.Core.Interfaces;
// Kısa mimari özet: Bu arayüz, SSE yayın mekanizmasının servisler ve controllerlar tarafından ortak kullanımını tanımlar.


public interface ISseBroadcaster
{
    Task BroadcastUpdateAsync(string eventName, string data = "", CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetEventStream(CancellationToken cancellationToken);
}
