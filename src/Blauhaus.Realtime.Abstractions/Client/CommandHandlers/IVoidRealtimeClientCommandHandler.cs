using Blauhaus.Domain.Abstractions.CommandHandlers;

namespace Blauhaus.Realtime.Abstractions.Client.CommandHandlers
{
    public interface IVoidRealtimeClientCommandHandler<TCommand> : IVoidCommandHandler<TCommand> 
        where TCommand : notnull
    {
        
    }
}