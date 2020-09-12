using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.ConnectionProxy
{
    public class SignalrServerConnectionProxy : ISignalrServerConnectionProxy
    {
        private IRealtimeClientConfig _config;
        private HubConnection? _hub;
         
        public void Configure(IRealtimeClientConfig config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken token)
        {
            if (_hub == null)
            {
                var builder = new HubConnectionBuilder()
                    .WithAutomaticReconnect();

                if (string.IsNullOrEmpty(_config.AccessToken))
                {
                    builder.WithUrl(_config.Url);
                }
                else
                {
                    builder.WithUrl(_config.Url, options => options.AccessTokenProvider = () => Task.FromResult(_config.AccessToken));
                }
            
                _hub = builder.Build();

                _hub.Reconnecting += OnReconnecting;
                _hub.Reconnected += OnReconnected;
                _hub.Closed += OnClosed;
            }

            await _hub.StartAsync(token);
        }

        public Task<ApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter)
        {
            return _hub.InvokeAsync<ApiResult<TResponse>>(methodName, parameter);
        }

        public Task<ApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter1, object parameter2)
        {
            return _hub.InvokeAsync<ApiResult<TResponse>>(methodName, parameter1, parameter2);
        }

        public Task<ApiResult> InvokeAsync(string methodName, object parameter)
        {
            return _hub.InvokeAsync<ApiResult>(methodName, parameter);
        }

        public Task<ApiResult> InvokeAsync(string methodName, object parameter1, object parameter2)
        {
            return _hub.InvokeAsync<ApiResult>(methodName, parameter1, parameter2);
        }

        public IDisposable On<T>(string methodName, Action<T> handler) => _hub.On(methodName, handler);

        public event EventHandler<ClientConnectionStateChangeEventArgs>? StateChanged;
        private Task OnClosed(Exception e)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, e));
            return Task.CompletedTask;
        }

        private Task OnReconnected(string arg)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connected, null));
            return Task.CompletedTask;
        }

        private Task OnReconnecting(Exception e)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Reconnecting, e));
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