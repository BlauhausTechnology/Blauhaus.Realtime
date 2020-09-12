using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;

namespace Blauhaus.Realtime.Server.SignalR.ConnectionProxy
{
    public class SignalrClientConnectionProxy : ISignalrClientConnectionProxy
    {
        private readonly HubCallerContext _context;

        public SignalrClientConnectionProxy(HubCallerContext context)
        {
            _context = context;
        }

        public string ConnectionId => _context.ConnectionId;
        public string UserIdentifier => _context.UserIdentifier;
        public ClaimsPrincipal User => _context.User;
        public IDictionary<object, object> Items => _context.Items;
        public IFeatureCollection Features => _context.Features;
        public CancellationToken ConnectionAborted => _context.ConnectionAborted;
    }
}