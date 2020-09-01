using System;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Client;

namespace Blauhaus.Realtime.Client.SignalR.Client
{
    public class RealtimeClient<TConfig> : IRealtimeClient<TConfig> where TConfig : IRealtimeClientConfig
    {
        private readonly IServiceLocator _serviceLocator;

        public RealtimeClient(
            IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }


        public IObservable<RealtimeClientState> Connect()
        {
            throw new NotImplementedException();
        }
    }
}