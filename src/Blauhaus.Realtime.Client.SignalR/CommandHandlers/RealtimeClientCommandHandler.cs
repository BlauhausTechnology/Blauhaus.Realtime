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
        private readonly IRealtimeClient _realtimeClient;


        public RealtimeClientCommandHandler(
            IAnalyticsService analyticsService,
            IClientRepository<TModel, TDto> clientRepository,
            IRealtimeClient realtimeClient)
        {
            _analyticsService = analyticsService;
            _clientRepository = clientRepository;
            _realtimeClient = realtimeClient;
        }


        public async Task<Result<TModel>> HandleAsync(TCommand command, CancellationToken token)
        {
            using (var _ = _analyticsService.StartOperation(this, typeof(TCommand).Name))
            {

                var properties = (Dictionary<string, string>)_analyticsService.AnalyticsOperationHeaders;

                var result = await _realtimeClient.InvokeAsync<TDto>("HandleUserSessionUpdateCommandAsync",  command, properties);


                return Result.Failure<TModel>("oops");
            }
        }
    }
}