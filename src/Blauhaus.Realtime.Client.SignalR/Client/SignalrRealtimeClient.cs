using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.Extensions;
using Blauhaus.Realtime.Client.SignalR.HubProxy;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.Client
{
    public class SignalrRealtimeClient :  IRealtimeClient
    {
        private readonly IRealtimeClientConfig _config;
        private readonly IAnalyticsService _analyticsService;
        private readonly IServiceLocator _serviceLocator;
        private IHubConnectionProxy? _hub;

        public SignalrRealtimeClient(
            IRealtimeClientConfig config,
            IAnalyticsService analyticsService,
            IServiceLocator serviceLocator)
        {
            _config = config;
            _analyticsService = analyticsService;
            _serviceLocator = serviceLocator;
        }


        public IObservable<RealtimeClientState> Observe()
        {
            return Observable.Create<RealtimeClientState>(async observer =>
            {
                var subscriptions = new CompositeDisposable();

                var hub = await GetHubAsync();
                observer.OnNext(hub.CurrentState.ToRealtimeClientState());

                void OnHubStateChanged(object sender, HubStateChangeEventArgs eventArgs)
                {

                    var clientState = eventArgs.State.ToRealtimeClientState();
                    var exception = eventArgs.Exception;
                    var traceMessage = $"SignalR client hub {clientState}";
                    
                    observer.OnNext(clientState);

                    if (clientState == RealtimeClientState.Reconnecting && exception != null)
                    {
                        _analyticsService.TraceWarning(this, $"{traceMessage} due to exception: {exception.Message}");
                    }

                    else if (clientState == RealtimeClientState.Disconnected)
                    {
                        _analyticsService.TraceWarning(this, traceMessage);
                        if (exception != null)
                        {
                            _analyticsService.LogException(this, exception);
                        }
                    }

                    else
                    {
                        _analyticsService.Trace(this, traceMessage);
                    }

                }

                subscriptions.Add(Observable.FromEventPattern(
                    x => hub.StateChanged += OnHubStateChanged, 
                    x => hub.StateChanged -= OnHubStateChanged).Subscribe());


                return subscriptions;
            });
        }

        public IObservable<TEvent> Connect<TEvent>(string methodName)
        {
            throw new NotImplementedException();
        }

        

        private async ValueTask<IHubConnectionProxy> GetHubAsync()
        {
            if (_hub == null)
            {
                _hub = await InitializeHubAsync();
                _analyticsService.TraceVerbose(this, "SignalR client hub initialized for the first time", _config.HubUrl.ToObjectDictionary("Url"));
            }

            return _hub;
        }

        private async Task<IHubConnectionProxy> InitializeHubAsync()
        {
            var hub = _serviceLocator.Resolve<IHubConnectionProxy>();
            hub.Initialize(new HubConnectionConfig
            {
                AccessToken = _config.AccessToken,
                Url = _config.HubUrl
            });

            await hub.StartAsync(CancellationToken.None);
            return hub;
        }

    }
}