using System;
using System.Collections.Generic;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Realtime.Abstractions.Client;
using Blauhaus.Realtime.Abstractions.Errors;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Client.SignalR.Client
{
    public class SignalrClientFactory : IRealtimeClientFactory
    {
        
        private IAnalyticsService AnalyticsService => _serviceLocator.Resolve<IAnalyticsService>();         //cannot inject from constructor as it is scoped while the factory is a singleton
        
        private readonly IServiceLocator _serviceLocator;
        private readonly IRealtimeClientDefinitions _clientDefinitions;
        private Dictionary<string, IRealtimeClientConfig> _runtimeClientDefinitions = new Dictionary<string, IRealtimeClientConfig>();
        private Dictionary<string, IRealtimeClient> _clients = new Dictionary<string, IRealtimeClient>();

        public SignalrClientFactory(
            IServiceLocator serviceLocator,
            IRealtimeClientDefinitions clientDefinitions)
        { 
            _serviceLocator = serviceLocator;
            _clientDefinitions = clientDefinitions;
        }


        public Result AddRuntimeClient(string clientName, IRealtimeClientConfig config)
        {
            _runtimeClientDefinitions[clientName] = config;
            return Result.Success();
        }

        public Result<IRealtimeClient> GetClient(string clientName = "")
        {

            if (_clients.TryGetValue(clientName, out var existingClient))
            {
                return Result.Success(existingClient);
            }

            if (!_runtimeClientDefinitions.TryGetValue(clientName, out var clientConfig))
            {
                clientConfig = _clientDefinitions.GetConfig(clientName);
                if (clientConfig == null)
                {
                    return AnalyticsService.TraceErrorResult<IRealtimeClient>(this, RealtimeErrors.NoClientConfiguration(clientName));
                }
            }

            var client = _serviceLocator.Resolve<IRealtimeClient>();
            client.Configure(clientConfig);

            AnalyticsService.TraceInformation(this, $"New {clientName} realtime client created");

            _clients[clientName] = client;

            return Result.Success(client);
        }
    }
}