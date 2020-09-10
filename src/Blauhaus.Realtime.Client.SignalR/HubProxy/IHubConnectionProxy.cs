using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Realtime.Abstractions.Server;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.HubProxy
{
    public interface IHubConnectionProxy
    {
        void Initialize(HubConnectionConfig config);

        HubConnectionState CurrentState { get; }
        string ConnectionId { get; }

        IDisposable On<T>(string methodName, Action<T> handler);

        Task StartAsync(CancellationToken token);

        Task<RealtimeApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter);
        Task<RealtimeApiResult> InvokeAsync(string methodName, object parameter);

        Task StopAsync(CancellationToken token);

        event EventHandler<HubStateChangeEventArgs> StateChanged;

        Task DisposeAsync();
    }
}