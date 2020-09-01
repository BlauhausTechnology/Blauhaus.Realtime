using System;
using System.Threading;
using System.Threading.Tasks;
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
        Task StopAsync(CancellationToken token);

        event EventHandler<HubStateChangeEventArgs> StateChanged;

        Task DisposeAsync();
    }
}