using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Realtime.Abstractions.Common;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.HubProxy
{
    public interface ISignalrServerConnectionProxy
    {
        void Initialize(HubConnectionConfig config);

        HubConnectionState CurrentState { get; }
        string ConnectionId { get; }

        IDisposable On<T>(string methodName, Action<T> handler);

        Task StartAsync(CancellationToken token);

        Task<ApiResult<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter, Dictionary<string, string> properties);
        Task<ApiResult> InvokeAsync(string methodName, object parameter, Dictionary<string, string> properties);

        Task StopAsync(CancellationToken token);

        event EventHandler<HubStateChangeEventArgs> StateChanged;

        Task DisposeAsync();
    }
}