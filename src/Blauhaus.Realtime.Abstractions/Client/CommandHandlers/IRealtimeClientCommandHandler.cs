using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Domain.Abstractions.Entities;

namespace Blauhaus.Realtime.Abstractions.Client.CommandHandlers
{
    public interface IRealtimeClientCommandHandler<TModel, TCommand> : ICommandHandler<TModel, TCommand> 
        where TCommand : notnull
        where TModel : class, IClientEntity 
    {
        
    }
}