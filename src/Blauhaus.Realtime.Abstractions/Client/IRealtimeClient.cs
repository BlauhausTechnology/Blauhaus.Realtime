using System;

namespace Blauhaus.Realtime.Abstractions.Client
{
     
    public interface IRealtimeClient
    {
        IObservable<RealtimeClientState> Observe();
        IObservable<TEvent> Connect<TEvent>(string methodName);
    }
}