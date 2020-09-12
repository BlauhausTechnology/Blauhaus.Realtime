using System.Collections.Generic;

namespace Blauhaus.Realtime.Abstractions.Client
{
    public interface IRealtimeClientDefinitions
    {
        IRealtimeClientConfig? GetConfig(string clientName = "");

    }
}