using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Realtime.Server.SignalR.CommandProcessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Realtime.Server.SignalR._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRealtimeCommandHandler<TPayload, TCommand, TCommandHandler>(this IServiceCollection services) 
            where TCommand : notnull 
            where TCommandHandler : class, ICommandHandler<TPayload, TCommand>
        {
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            services.TryAddScoped<ICommandHandler<TPayload, TCommand>, TCommandHandler>();

            return services;
        }

        public static IServiceCollection AddRealtimeAuthenticatedCommandHandler<TPayload, TUser, TCommand, TCommandHandler>(this IServiceCollection services)
            where TCommand : notnull 
            where TCommandHandler : class, IAuthenticatedCommandHandler<TPayload, TCommand, TUser>
            where TUser : notnull
        {
            
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            services.TryAddScoped<IAuthenticatedCommandHandler<TPayload, TCommand, TUser>, TCommandHandler>();

            return services;
        }


        public static IServiceCollection AddVoidRealtimeCommandHandler<TCommand>(this IServiceCollection services)
        {
            

            return services;
        }

        public static IServiceCollection AddVoidRealtimeAuthenticatedCommandHandler<TCommand, TUser>(this IServiceCollection services)
        {
            

            return services;
        }
    }
}