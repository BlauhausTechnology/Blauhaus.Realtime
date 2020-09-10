using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Common;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.HubProxy
{
    public class SignalrServerConnectionProxy : ISignalrServerConnectionProxy
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

        public Task<ApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter, Dictionary<string, string> properties)
        {
            return _hub.InvokeAsync<ApiResult<TResponse>>(methodName, parameter, properties);
        }

        public Task<ApiResult> InvokeAsync(string methodName, object parameter, Dictionary<string, string> properties)
        {
            return _hub.InvokeAsync<ApiResult>(methodName, parameter, properties);
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