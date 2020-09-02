using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.HubProxy;

namespace Blauhaus.Realtime.Client.SignalR.Client._Base
{
    public abstract class BaseRealtimeClient<TConfig> : IRealtimeClient<TConfig> where TConfig : IRealtimeClientConfig
    {
        private readonly TConfig _config;
        private readonly IServiceLocator _serviceLocator;
        private IHubConnectionProxy _hub;

        protected BaseRealtimeClient(
            TConfig config,
            IServiceLocator serviceLocator)
        {
            _config = config;
            _serviceLocator = serviceLocator;
        }

        public IObservable<RealtimeClientState> Connect()
        {
            return Observable.Create<RealtimeClientState>(observer =>
            {
                var disposable = new CompositeDisposable();

                _hub = _serviceLocator.Resolve<IHubConnectionProxy>();
                _hub.Initialize(new HubConnectionConfig
                {
                    Url = _config.HubUrl
                });

                return disposable;
            });
        }
    }
}