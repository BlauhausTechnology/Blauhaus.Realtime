using System;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.Realtime.Client.SignalR.HubProxy
{
    public class HubStateChangeEventArgs : EventArgs
    {
        public HubStateChangeEventArgs(HubConnectionState state, Exception? exception)
        {
            State = state;
            Exception = exception;
        }

        public HubConnectionState State { get; }
        public Exception? Exception { get; }


    }
}