using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Server;

namespace Blauhaus.Realtime.Server.SignalR.CommandProcessor
{
    public class RealtimeCommandProcessor : IRealtimeCommandProcessor
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IServiceLocator _serviceLocator;
        private readonly IAuthenticatedUserFactory _userFactory;

        public RealtimeCommandProcessor(
            IAnalyticsService analyticsService,
            IServiceLocator serviceLocator,
            IAuthenticatedUserFactory userFactory)
        {
            _analyticsService = analyticsService;
            _serviceLocator = serviceLocator;
            _userFactory = userFactory;
        }


        public async Task<RealtimeApiResult<TPayload>> HandleAsync<TPayload, TCommand>(TCommand command) where TCommand : notnull
        {
            try
            {
                var commandHandler = _serviceLocator.Resolve<ICommandHandler<TPayload, TCommand>>();
                var result = await commandHandler.HandleAsync(command, CancellationToken.None);
                if (result.IsSuccess)
                {
                    return new RealtimeApiResult<TPayload>(true, result.Value);
                }
                else
                {
                    return new RealtimeApiResult<TPayload>(false, default!);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<RealtimeApiResult<TPayload>> HandleAuthenticatedAsync<TPayload, TCommand>(TCommand command, ClaimsPrincipal user) where TCommand : notnull
        {
            try
            {
                var commandHandler = _serviceLocator.Resolve<IAuthenticatedCommandHandler<TPayload, TCommand, IAuthenticatedUser>>();
                var authenticatedUserResult = _userFactory.Create(user);
                if (authenticatedUserResult.IsFailure)
                {
                    return new RealtimeApiResult<TPayload>(authenticatedUserResult);
                }

                var result = await commandHandler.HandleAsync(command, authenticatedUserResult.Value, CancellationToken.None);

                return new RealtimeApiResult<TPayload>(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task<RealtimeApiResult> HandleVoidAsync<TCommand>(TCommand command) where TCommand : notnull
        {
            throw new NotImplementedException();
        }

        public Task<RealtimeApiResult> HandleVoidAuthenticatedAsync<TCommand>(TCommand command, ClaimsPrincipal user) where TCommand : notnull
        {
            throw new NotImplementedException();
        }
    }
}