using System;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.Client._Base;
using Blauhaus.Realtime.Client.SignalR.HubProxy;

namespace Blauhaus.Realtime.Client.SignalR.Client
{
    public class RealtimeClient<TConfig> : BaseRealtimeClient<TConfig> where TConfig : IRealtimeClientConfig
    {
        public RealtimeClient(TConfig config, IServiceLocator serviceLocator) : base(config, serviceLocator)
        {
        }
    }
}