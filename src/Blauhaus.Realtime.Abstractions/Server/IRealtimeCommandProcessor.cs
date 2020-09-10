using System.Security.Claims;
using System.Threading.Tasks;

namespace Blauhaus.Realtime.Abstractions.Server
{
    public interface IRealtimeCommandProcessor
    {
        Task<RealtimeApiResult<TPayload>> HandleAsync<TPayload, TCommand>(TCommand command) where TCommand : notnull;
        Task<RealtimeApiResult<TPayload>> HandleAuthenticatedAsync<TPayload, TCommand>(TCommand command, ClaimsPrincipal user) where TCommand : notnull;
        Task<RealtimeApiResult> HandleVoidAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<RealtimeApiResult> HandleVoidAuthenticatedAsync<TCommand>(TCommand command, ClaimsPrincipal user) where TCommand : notnull;
    }
}