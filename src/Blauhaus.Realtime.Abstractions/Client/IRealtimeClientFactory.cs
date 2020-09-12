using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Abstractions.Client
{
    public interface IRealtimeClientFactory
    {
        Result AddRuntimeClient(string clientName, IRealtimeClientConfig config);
        Result<IRealtimeClient> GetClient(string clientName = "");
    }
}