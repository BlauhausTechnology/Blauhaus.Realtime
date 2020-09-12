namespace Blauhaus.Realtime.Abstractions.Client
{
    public interface IRealtimeClientConfig
    {
        public string Url { get; set; }
        public string? AccessToken { get; set; }
    }
}