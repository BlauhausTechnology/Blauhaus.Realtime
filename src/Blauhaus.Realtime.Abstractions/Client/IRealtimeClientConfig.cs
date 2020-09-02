namespace Blauhaus.Realtime.Abstractions.Client
{
    public interface IRealtimeClientConfig
    {
        public string HubUrl { get; }
        public string? AccessToken { get; }
    }
}