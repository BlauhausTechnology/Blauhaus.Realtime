using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.Client;
using Blauhaus.Realtime.Client.SignalR.HubProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Realtime.Client.SignalR._Ioc
{
    public static class ServiceCollectionExtensions
    {
         
        public static IServiceCollection AddRealtimeClient<TConfig>(this IServiceCollection services) 
            where TConfig : class, IRealtimeClientConfig
        {
            services.AddTransient<IRealtimeClientConfig, TConfig>();
            services.TryAddTransient<ISignalrServerConnectionProxy, SignalrServerConnectionProxy>();
            services.AddSingleton<IRealtimeClient, SignalrRealtimeClient>();

            return services;
        }
         
    }
}