namespace Bitjuice.EmberZNet;

public interface IEzspChannel
{
    Task ConnectAsync(IEzspCallbackHandler handler, CancellationToken cancellationToken);
    Task DisconnectAsync();
    Task<TResponse> SendAsync<TResponse>(EzspCommand cmd);
    Task<TResponse> SendAsync<TRequest, TResponse>(EzspCommand cmd, TRequest request);
    Task<ReadOnlyMemory<byte>> SendAsync(EzspCommand cmd, params byte[] data);
}