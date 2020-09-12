using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Common;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.ConnectionProxy
{
    public interface ISignalrServerConnectionProxy
    {
        HubConnectionState CurrentState { get; }
        string ConnectionId { get; }

        IDisposable On<T>(string methodName, Action<T> handler);

        void Configure(IRealtimeClientConfig config);

        Task StartAsync(CancellationToken token);

        Task<ApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter);
        Task<ApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter1, object parameter2);
        Task<ApiResult> InvokeAsync(string methodName, object parameter);
        Task<ApiResult> InvokeAsync(string methodName, object parameter1, object parameter2);

        Task StopAsync(CancellationToken token);

        event EventHandler<ClientConnectionStateChangeEventArgs> StateChanged;

        Task DisposeAsync();
    }
}