using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Server;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.HubProxy
{
    public class HubConnectionProxy : IHubConnectionProxy
    {
        private HubConnection _hub;
        
        public void Initialize(HubConnectionConfig config)
        {
            var builder = new HubConnectionBuilder()
                .WithAutomaticReconnect();

            if (string.IsNullOrEmpty(config.AccessToken))
            {
                builder.WithUrl(config.Url);
            }
            else
            {
                builder.WithUrl(config.Url, options => options.AccessTokenProvider = () => Task.FromResult(config.AccessToken));
            }
            
            _hub = builder.Build();

            _hub.Reconnecting += OnReconnecting;
            _hub.Reconnected += OnReconnected;
            _hub.Closed += OnClosed;

        }

        public Task StartAsync(CancellationToken token) => _hub.StartAsync(token);

        public Task<RealtimeApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter)
        {
            return _hub.InvokeAsync<RealtimeApiResult<TResponse>>(methodName, parameter);
        }

        public Task<RealtimeApiResult> InvokeAsync(string methodName, object parameter)
        {
            return _hub.InvokeAsync<RealtimeApiResult>(methodName, parameter);
        }

        public IDisposable On<T>(string methodName, Action<T> handler) => _hub.On(methodName, handler);

        public event EventHandler<HubStateChangeEventArgs>? StateChanged;
        private Task OnClosed(Exception e)
        {
            StateChanged?.Invoke(this, new HubStateChangeEventArgs(HubConnectionState.Disconnected, e));
            return Task.CompletedTask;
        }

        private Task OnReconnected(string arg)
        {
            StateChanged?.Invoke(this, new HubStateChangeEventArgs(HubConnectionState.Connected, null));
            return Task.CompletedTask;
        }

        private Task OnReconnecting(Exception e)
        {
            StateChanged?.Invoke(this, new HubStateChangeEventArgs(HubConnectionState.Reconnecting, e));
            return Task.CompletedTask;
        }

        public HubConnectionState CurrentState => _hub.State;
        public string ConnectionId => _hub.ConnectionId;
        public Task StopAsync(CancellationToken token) => _hub.StopAsync(token);
        public Task DisposeAsync()
        {
            _hub.Reconnecting -= OnReconnecting;
            _hub.Reconnected -= OnReconnected;
            _hub.Closed -= OnClosed;

            return _hub.DisposeAsync();
        }
    }
}