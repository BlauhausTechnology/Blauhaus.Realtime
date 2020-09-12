using Blauhaus.Realtime.Abstractions.Client;

namespace Blauhaus.Realtime.Client.SignalR._Ioc
{
    public class DummyClientDefinitions : IRealtimeClientDefinitions
    {
        public IRealtimeClientConfig? GetConfig(string clientName = "")
        {
            return null;
        }
    }
}