using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Client.SignalR.Client;
using Blauhaus.Realtime.Client.SignalR.ConnectionProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Realtime.Client.SignalR._Ioc
{
    public static class ServiceCollectionExtensions
    {
         
        public static IServiceCollection AddSignalrClient(this IServiceCollection services)  
        {
            return services.AddSignalrClient<DummyClientDefinitions>();
        }
        public static IServiceCollection AddSignalrClient<TConfig>(this IServiceCollection services) 
            where TConfig : class, IRealtimeClientDefinitions
        {
            services.AddTransient<IRealtimeClientDefinitions, TConfig>();
            services.TryAddTransient<ISignalrServerConnectionProxy, SignalrServerConnectionProxy>();
            services.AddTransient<IRealtimeClient, SignalrClient>();
            services.AddSingleton<IRealtimeClientFactory, SignalrClientFactory>();

            return services;
        }
         
    }
}