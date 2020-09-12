using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Repositories;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Client.CommandHandlers;
using Blauhaus.Realtime.Abstractions.Common;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Client.SignalR.CommandHandlers
{
    public class RealtimeClientCommandHandler<TModel, TDto, TCommand> : IRealtimeClientCommandHandler<TModel, TCommand>
        where TCommand : notnull
        where TModel : class, IClientEntity
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IClientRepository<TModel, TDto> _clientRepository;
        private readonly IRealtimeClientFactory _clientFactory;


        public RealtimeClientCommandHandler(
            IAnalyticsService analyticsService,
            IClientRepository<TModel, TDto> clientRepository,
            IRealtimeClientFactory clientFactory)
        {
            _analyticsService = analyticsService;
            _clientRepository = clientRepository;
            _clientFactory = clientFactory;
        }


        public async Task<Result<TModel>> HandleAsync(TCommand command, CancellationToken token)
        {
            //todo if TCommand is ICommandWithClientName then resolve named client

            var client = _clientFactory.GetClient();

            var result = await client.Value.InvokeAsync<TDto>("HandleUserSessionUpdateCommandAsync",  command);


            return Result.Failure<TModel>("oops");
        }
    }
}