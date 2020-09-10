using System.Threading.Tasks;
using Blauhaus.Ioc.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Blauhaus.Realtime.Server.SignalR.Hubs
{
    public class RealtimeHub :  Hub
    {
        private readonly IServiceLocator _serviceLocator;

        public RealtimeHub(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }
         


    }
}