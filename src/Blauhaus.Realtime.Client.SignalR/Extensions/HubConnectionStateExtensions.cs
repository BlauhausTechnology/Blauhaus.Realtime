using System;
using Blauhaus.Realtime.Abstractions.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.Extensions
{
    internal static class HubConnectionStateExtensions
    {
        internal static RealtimeClientState ToRealtimeClientState(this HubConnectionState state)
        {
            switch (state)
            {
                case HubConnectionState.Connecting:
                    return RealtimeClientState.Connecting;
                case HubConnectionState.Connected:
                    return RealtimeClientState.Connected;
                case HubConnectionState.Disconnected:
                    return RealtimeClientState.Disconnected;
                case HubConnectionState.Reconnecting:
                    return RealtimeClientState.Reconnecting;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}