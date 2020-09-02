using System;

namespace Blauhaus.Realtime.Abstractions.Client
{

    public interface IRealtimeClient<TConfig> : IRealtimeClient
        where TConfig : IRealtimeClientConfig
    {

    }

    public interface IRealtimeClient 
    {
        IObservable<RealtimeClientState> Connect();
    }
}