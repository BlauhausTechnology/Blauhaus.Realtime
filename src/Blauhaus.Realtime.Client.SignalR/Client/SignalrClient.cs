using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Client.SignalR.ConnectionProxy;
using Blauhaus.Realtime.Client.SignalR.Extensions;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.Client
{
    public class SignalrClient :  IRealtimeClient
    {
        private IRealtimeClientConfig _config;
        private readonly IAnalyticsService _analyticsService;
        private readonly IServiceLocator _serviceLocator;
        private ISignalrServerConnectionProxy? _hub;

        public SignalrClient(
            IAnalyticsService analyticsService,
            IServiceLocator serviceLocator)
        {
            _analyticsService = analyticsService;
            _serviceLocator = serviceLocator;
        }


        public void Configure(IRealtimeClientConfig config)
        {
            _config = config;
        }

        public IObservable<RealtimeClientState> Observe()
        {
            return Observable.Create<RealtimeClientState>(async observer =>
            {
                var subscriptions = new CompositeDisposable();

                var hub = await GetHubAsync();
                observer.OnNext(hub.CurrentState.ToRealtimeClientState());

                void OnHubStateChanged(object sender, ClientConnectionStateChangeEventArgs eventArgs)
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

        public async Task<Result<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter)
        {
            var hubResult = await GetHubAsync();

            var commandResult = await hubResult.InvokeAsync<TResponse>(methodName, parameter, _analyticsService.AnalyticsOperationHeaders);

            if (commandResult.IsFailure) return Result.Failure<TResponse>(commandResult.Error);

            else return Result.Success(commandResult.Value);
        }

        public Task<Result> InvokeAsync(string methodName, object parameter)
        {
            throw new NotImplementedException();
        }


        private async ValueTask<ISignalrServerConnectionProxy> GetHubAsync()
        {
            if (_hub == null)
            {
                var hub = _serviceLocator.Resolve<ISignalrServerConnectionProxy>();
                hub.Configure(_config);

                await hub.StartAsync(CancellationToken.None);
                _analyticsService.TraceVerbose(this, "SignalR client hub initialized for the first time", _config.Url.ToObjectDictionary("Url"));
                
                _hub = hub;
            }

            return _hub;
        }
         

    }
}