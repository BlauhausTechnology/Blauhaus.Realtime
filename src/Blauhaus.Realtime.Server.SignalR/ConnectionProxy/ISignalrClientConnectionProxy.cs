using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http.Features;

namespace Blauhaus.Realtime.Server.SignalR.ConnectionProxy
{
    public interface ISignalrClientConnectionProxy
    {
        public string ConnectionId { get; }

        public string UserIdentifier { get; }

        public ClaimsPrincipal User { get; }

        public IDictionary<object, object> Items { get; }

        public IFeatureCollection Features { get; }

        public CancellationToken ConnectionAborted { get; }
    }
}