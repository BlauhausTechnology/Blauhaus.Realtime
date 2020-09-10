using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Realtime.Abstractions.Common;
using Blauhaus.Realtime.Server.SignalR.Hubs;

namespace Blauhaus.Realtime.Server.SignalR.CommandProcessor
{
    public interface ISignalrCommandProcessor
    {
        Task<ApiResult> HandleVoidCommandAsync<TCommand>(TCommand command, Dictionary<string,string> properties, ISignalrClientConnectionProxy clientConnection) where TCommand : notnull;
        Task<ApiResult> HandleVoidAuthenticatedCommandAsync<TCommand>(TCommand command, Dictionary<string,string> properties, ISignalrClientConnectionProxy clientConnection) where TCommand : notnull;

        Task<ApiResult<TPayload>> HandleCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string,string> properties, ISignalrClientConnectionProxy clientConnection) where TCommand : notnull;
        Task<ApiResult<TPayload>> HandleAuthenticatedCommandAsync<TPayload, TCommand>(TCommand command, Dictionary<string,string> properties, ISignalrClientConnectionProxy clientConnection) where TCommand : notnull;
        }
}