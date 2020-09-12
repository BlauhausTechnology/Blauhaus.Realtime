using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Http;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.ConnectionProxy;

namespace Blauhaus.Realtime.Server.SignalR.CommandProcessor
{
    public class SignalrCommandProcessor : ISignalrCommandProcessor
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IAuthenticatedUserFactory _authenticatedUserFactory;
        private readonly IServiceLocator _serviceLocator;

        public SignalrCommandProcessor(
            IAnalyticsService analyticsService,
            IAuthenticatedUserFactory authenticatedUserFactory,
            IServiceLocator serviceLocator)
        {
            _analyticsService = analyticsService;
            _authenticatedUserFactory = authenticatedUserFactory;
            _serviceLocator = serviceLocator;
        }
        
        public async Task<ApiResult> HandleVoidCommandAsync<TCommand>(TCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        { 
            properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

            using (var _ = _analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
            {
                _analyticsService.TraceVerbose(this, $"{typeof(TCommand).Name} received", command.ToObjectDictionary("Command"));

                try
                {
                    var commandHandler = _serviceLocator.Resolve<IVoidCommandHandler<TCommand>>();
                    var handleCommandResult =  await commandHandler.HandleAsync(command, signalrHubProxy.ConnectionAborted);
                    return new ApiResult(handleCommandResult);
                }
                catch (Exception e)
                {
                    _analyticsService.LogException(this, e);
                    return new ApiResult(false, ApiErrors.UnhandledServerError.ToString());
                }
            }
        }

        public async Task<ApiResult> HandleVoidAuthenticatedCommandAsync<TCommand>(TCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        { 
            properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

            using (var _ =_analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
            {
                _analyticsService.TraceVerbose(this, $"{typeof(TCommand).Name} received", command.ToObjectDictionary("Command"));

                try
                {
                    var authenticationResult = _authenticatedUserFactory.Create(signalrHubProxy.User);
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
                    _analyticsService.LogException(this, e);
                    return new ApiResult(false, ApiErrors.UnhandledServerError.ToString());
                }
            }
        }

        public async Task<ApiResult<TPayload>> HandleCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string, string> properties,  ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        { 
            properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

            using (var _ = _analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
            {
                _analyticsService.TraceVerbose(this, $"{typeof(TCommand).Name} received", command.ToObjectDictionary("Command"));

                try
                {
                    var commandHandler = _serviceLocator.Resolve<ICommandHandler<TPayload, TCommand>>();
                    var handleCommandResult = await commandHandler.HandleAsync(command, signalrHubProxy.ConnectionAborted);
                    return new ApiResult<TPayload>(handleCommandResult);
                }
                catch (Exception e)
                {
                    _analyticsService.LogException(this, e);
                    return new ApiResult<TPayload>(false, default!, ApiErrors.UnhandledServerError.ToString());
                }
            }
        }

        public async Task<ApiResult<TPayload>> HandleAuthenticatedCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string, string> properties, ISignalrClientConnectionProxy signalrHubProxy) where TCommand : notnull
        { 
            properties[$"{AnalyticsHeaders.Prefix}ConnectionId"] = signalrHubProxy.ConnectionId;

            using (var _ = _analyticsService.StartRequestOperation(this, typeof(TCommand).Name, properties))
            {
                _analyticsService.TraceVerbose(this, $"{typeof(TCommand).Name} received", command.ToObjectDictionary("Command"));

                try
                { 
                    var authenticationResult = _authenticatedUserFactory.Create(signalrHubProxy.User);
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
                    _analyticsService.LogException(this, e);
                    return new ApiResult<TPayload>(false, default!, ApiErrors.UnhandledServerError.ToString());
                }
            }
        }

    }
}