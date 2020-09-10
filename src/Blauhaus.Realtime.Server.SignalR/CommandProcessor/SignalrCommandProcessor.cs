using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Http;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.Hubs;

namespace Blauhaus.Realtime.Server.SignalR.CommandProcessor
{
    public class SignalrCommandProcessor : ISignalrCommandProcessor
    {
        private readonly IServiceLocator _serviceLocator;

        public SignalrCommandProcessor(
            IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }
        
        public async Task<ApiResult> HandleVoidCommandAsync<TCommand>(TCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        {
            using (var _ = _serviceLocator.ResetScope())
            {
                var analyticsService = _serviceLocator.Resolve<IAnalyticsService>();
                properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

                using (var operation = analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
                {
                    try
                    {
                        var commandHandler = _serviceLocator.Resolve<IVoidCommandHandler<TCommand>>();
                        var handleCommandResult =  await commandHandler.HandleAsync(command, signalrHubProxy.ConnectionAborted);
                         return new ApiResult(handleCommandResult);
                    }
                    catch (Exception e)
                    {
                        analyticsService.LogException(this, e);
                        return new ApiResult(false, ApiErrors.UnhandledServerError.ToString());
                    }
                }
            }
        }

        public async Task<ApiResult> HandleVoidAuthenticatedCommandAsync<TCommand>(TCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        {
            using (var _ = _serviceLocator.ResetScope())
            {
                var analyticsService = _serviceLocator.Resolve<IAnalyticsService>();
                properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

                using (var operation = analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
                {
                    try
                    {
                        var userFactory = _serviceLocator.Resolve<IAuthenticatedUserFactory>();
                        var authenticationResult = userFactory.Create(signalrHubProxy.User);
                        if (authenticationResult.IsFailure)
                        {
                            return new ApiResult(authenticationResult);
                        }

                        var commandHandler = _serviceLocator.Resolve<IVoidAuthenticatedCommandHandler<TCommand, IAuthenticatedUser>>();
                        var handleCommandResult = await commandHandler.HandleAsync(command, authenticationResult.Value, signalrHubProxy.ConnectionAborted);
                        return new ApiResult(handleCommandResult);
                    }
                    catch (Exception e)
                    {
                        analyticsService.LogException(this, e);
                        return new ApiResult(false, ApiErrors.UnhandledServerError.ToString());
                    }
                }
            }
        }

        public async Task<ApiResult<TPayload>> HandleCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string, string> properties,  ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        {
            using (var _ = _serviceLocator.ResetScope())
            { 
                var analyticsService = _serviceLocator.Resolve<IAnalyticsService>();
                properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

                using (var operation = analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
                {
                    try
                    {
                        var commandHandler = _serviceLocator.Resolve<ICommandHandler<TPayload, TCommand>>();
                        var handleCommandResult = await commandHandler.HandleAsync(command, signalrHubProxy.ConnectionAborted);
                        return new ApiResult<TPayload>(handleCommandResult);
                    }
                    catch (Exception e)
                    {
                        analyticsService.LogException(this, e);
                        return new ApiResult<TPayload>(false, default!, ApiErrors.UnhandledServerError.ToString());
                    }
                }
            }
        }

        public async Task<ApiResult<TPayload>> HandleAuthenticatedCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        {
            using (var scope = _serviceLocator.ResetScope())
            {
                var analyticsService = _serviceLocator.Resolve<IAnalyticsService>();
                properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

                using (var operation = analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
                {
                    try
                    {
                        var userFactory = _serviceLocator.Resolve<IAuthenticatedUserFactory>();
                        var authenticationResult = userFactory.Create(signalrHubProxy.User);
                        if (authenticationResult.IsFailure)
                        {
                            return new ApiResult<TPayload>(authenticationResult);
                        }

                        var commandHandler = _serviceLocator.Resolve<IAuthenticatedCommandHandler<TPayload, TCommand, IAuthenticatedUser>>(); 

                        var handleCommandResult = await commandHandler.HandleAsync(command, authenticationResult.Value, signalrHubProxy.ConnectionAborted);
                        return new ApiResult<TPayload>(handleCommandResult);
                    }
                    catch (Exception e)
                    {
                        analyticsService.LogException(this, e);
                        return new ApiResult<TPayload>(false, default!, ApiErrors.UnhandledServerError.ToString());
                    }
                }
            } 
        }

    }
}