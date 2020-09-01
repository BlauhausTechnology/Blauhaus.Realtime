using System;

namespace Blauhaus.Realtime.Abstractions.Client
{
    public interface IRealtimeClient<TConfig> where TConfig : IRealtimeClientConfig
    {
        IObservable<RealtimeClientState> Connect();
    }
}