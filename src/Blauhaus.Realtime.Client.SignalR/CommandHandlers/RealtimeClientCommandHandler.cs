using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Repositories;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Client.CommandHandlers;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Client.SignalR.CommandHandlers
{
    public class RealtimeClientCommandHandler<TModel, TModelDto, TCommand> : IRealtimeClientCommandHandler<TModel, TCommand>
        where TCommand : notnull
        where TModel : class, IClientEntity
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IClientRepository<TModel, TModelDto> _clientRepository;
        private readonly IRealtimeClient _realtimeClient;

        private IDisposable? _clientSubscription;

        public RealtimeClientCommandHandler(
            IAnalyticsService analyticsService,
            IClientRepository<TModel, TModelDto> clientRepository,
            IRealtimeClient realtimeClient)
        {
            _analyticsService = analyticsService;
            _clientRepository = clientRepository;
            _realtimeClient = realtimeClient;
        }


        public async Task<Result<TModel>> HandleAsync(TCommand command, CancellationToken token)
        {
            var result = await _realtimeClient.InvokeAsync<TModelDto>("HandleUserSessionUpdateCommandAsync", command);


            return Result.Failure<TModel>("oops");
        }
    }
}