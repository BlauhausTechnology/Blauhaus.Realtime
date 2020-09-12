using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.CommandProcessor;
using Blauhaus.Realtime.Server.SignalR.ConnectionProxy;
using Microsoft.AspNetCore.SignalR;

namespace Blauhaus.Realtime.Server.SignalR.Hubs
{
    public abstract class BaseSignalrHub<TClient> : Hub<TClient> where TClient : class
    {
        private readonly ISignalrCommandProcessor _commandProcessor;

        protected BaseSignalrHub(ISignalrCommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        protected Task<ApiResult> HandleVoidCommandAsync<TCommand>(TCommand command, Dictionary<string, string> properties) where TCommand : notnull
        {
            return _commandProcessor.HandleVoidCommandAsync(command, properties, new SignalrClientConnectionProxy(Context));
        }

        Task<ApiResult> HandleVoidAuthenticatedCommandAsync<TCommand>(TCommand command, Dictionary<string,string> properties) where TCommand : notnull
        {
            return _commandProcessor.HandleVoidAuthenticatedCommandAsync(command, properties, new SignalrClientConnectionProxy(Context));
        }

        Task<ApiResult<TPayload>> HandleCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string,string> properties) where TCommand : notnull
        {
            return _commandProcessor.HandleCommandAsync<TPayload, TCommand>(command, properties, new SignalrClientConnectionProxy(Context));
        }

        Task<ApiResult<TPayload>> HandleAuthenticatedCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string,string> properties) where TCommand : notnull
        {
            return _commandProcessor.HandleAuthenticatedCommandAsync<TPayload, TCommand>(command, properties, new SignalrClientConnectionProxy(Context));
        }
    }
}