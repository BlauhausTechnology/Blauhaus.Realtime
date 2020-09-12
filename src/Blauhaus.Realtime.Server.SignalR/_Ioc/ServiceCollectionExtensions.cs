using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Realtime.Server.SignalR.CommandProcessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.Realtime.Server.SignalR._Ioc
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddSignalRServer(this IServiceCollection services)
        {
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            return services;
        }

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

        public static IServiceCollection AddRealtimeAuthenticatedCommandHandler<TPayload, TCommand, TCommandHandler>(this IServiceCollection services)
            where TCommand : notnull 
            where TCommandHandler : class, IAuthenticatedCommandHandler<TPayload, TCommand, IAuthenticatedUser> 
        {
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            services.TryAddScoped<IAuthenticatedCommandHandler<TPayload, TCommand, IAuthenticatedUser>, TCommandHandler>();
            return services;
        }


        public static IServiceCollection AddVoidRealtimeCommandHandler<TCommand, TCommandHandler>(this IServiceCollection services) 
            where TCommandHandler : class, IVoidCommandHandler<TCommand>
            where TCommand : notnull
        {
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            services.TryAddScoped<IVoidCommandHandler<TCommand>, TCommandHandler>();
            return services;
        }

        public static IServiceCollection AddVoidRealtimeAuthenticatedCommandHandler<TCommand, TUser, TCommandHandler>(this IServiceCollection services) 
            where TCommandHandler : class, IVoidAuthenticatedCommandHandler<TCommand, TUser>
            where TCommand :  notnull
            where TUser : notnull
        {
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            services.TryAddScoped<IVoidAuthenticatedCommandHandler<TCommand, TUser>, TCommandHandler>();
            return services;
        }

        
        public static IServiceCollection AddVoidRealtimeAuthenticatedCommandHandler<TCommand, TCommandHandler>(this IServiceCollection services) 
            where TCommandHandler : class, IVoidAuthenticatedCommandHandler<TCommand, IAuthenticatedUser>
            where TCommand :  notnull 
        {
            services.TryAddScoped<ISignalrCommandProcessor, SignalrCommandProcessor>();
            services.TryAddScoped<IVoidAuthenticatedCommandHandler<TCommand, IAuthenticatedUser>, TCommandHandler>();
            return services;
        }
    }
}