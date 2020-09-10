using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Blauhaus.Realtime.Abstractions.Client
{
     
    public interface IRealtimeClient
    {
        IObservable<RealtimeClientState> Observe();
        IObservable<TEvent> Connect<TEvent>(string methodName);
        
        Task<Result<TResponse>> InvokeAsync<TResponse>(string methodName, object parameter, Dictionary<string, string> properties);
        Task<Result> InvokeAsync(string methodName, object parameter, Dictionary<string, string> properties);

    }
}