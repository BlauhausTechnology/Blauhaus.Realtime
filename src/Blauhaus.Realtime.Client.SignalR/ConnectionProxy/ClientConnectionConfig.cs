using Blauhaus.Realtime.Abstractions.Client;

namespace Blauhaus.Realtime.Client.SignalR.ConnectionProxy
{
    public class ClientConnectionConfig : IRealtimeClientConfig
    {
        public string Url { get; set; }
        public string? AccessToken { get; set; }
    }
}